import React, { useState, useEffect, useMemo } from 'react'
import {
  UserClient,
  UserTypeClient,
  UserDto,
  UserRequest,
  UserTypeDto,
  UserFilter,
} from '../api/apiClient.ts'
import {
  Layout,
  Table,
  Button,
  Modal,
  notification,
  Form,
  Input,
  Select,
} from 'antd'
import { UserAddOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons'

const { Content } = Layout

const AdminPage: React.FC = () => {

  // Users state
  const [users, setUsers] = useState<UserDto[]>([])
  const [userPage, setUserPage] = useState(1)
  const [userTotal, setUserTotal] = useState(0)
  const [userTypes, setUserTypes] = useState<UserTypeDto[]>([])
  const [userSearch, setUserSearch] = useState('')
  const [userPageSize, setUserPageSize] = useState(10)

  // Add modals
  const [isAddUserModalVisible, setIsAddUserModalVisible] = useState(false)
  const [addUserForm] = Form.useForm()

  // Edit modals
  const [isEditUserModalVisible, setIsEditUserModalVisible] = useState(false)
  const [editingUser, setEditingUser] = useState<UserDto | null>(null)
  const [userForm] = Form.useForm()

  const userClient = new UserClient()
  const userTypeClient = useMemo(() => new UserTypeClient(), [])

  useEffect(() => {
    const fetchUserTypes = async () => {
      try {
        const types = await userTypeClient.getAllUserTypes()
        const items = Array.isArray(types) ? types : types?.items || []
        setUserTypes(items)
      } catch {
        notification.error({
          message: 'Error',
          description: 'Error fetching user types',
          placement: 'topRight',
          duration: 2
        })
      }
    }
    fetchUserTypes()
  }, [userTypeClient])

  useEffect(() => {
    fetchUsers(userPage, userPageSize)
  }, [userPage, userPageSize])

  const fetchUsers = async (page = 1, pageSize = userPageSize) => {
    try {
      const filter = new UserFilter()
      filter.pageSize = pageSize
      filter.pageNumber = page
      if (userSearch) filter.name = userSearch
      const result = await userClient.getAllUsers(filter)
      const items = Array.isArray(result) ? result : result?.items || []
      const total =
        !Array.isArray(result) && typeof result?.totalCount === 'number'
          ? result.totalCount
          : items.length
      setUsers(items)
      setUserTotal(total)
    } catch {
      notification.error({
        message: 'Error',
        description: 'Error fetching users',
        placement: 'topRight',
        duration: 2
      })
    }
  }

  const handleAddUser = () => {
    addUserForm.resetFields()
    setIsAddUserModalVisible(true)
  }

  const handleAddUserSubmit = async () => {
    try {
      const values = await addUserForm.validateFields()

      const { password, ...userFields } = values
      const request = new UserRequest()
      request.user = { ...userFields }
      request.password = password

      await userClient.createNewUser({
        ...request,
        user: { ...request.user }
      })
      notification.success({
        message: 'Success',
        description: 'User added',
        placement: 'topRight',
        duration: 2
      })

      setIsAddUserModalVisible(false)
      await fetchUsers(userPage)
    } catch {
      notification.error({
        message: 'Error',
        description: 'Failed to add user',
        placement: 'topRight',
        duration: 2
      })
    }
  }

  const handleEditUser = (user: UserDto) => {
    setEditingUser(user)
    userForm.setFieldsValue(user)
    setIsEditUserModalVisible(true)
  }

  const handleUserEditSubmit = async () => {
    try {
      const values = await userForm.validateFields()
      if (editingUser) {
        const { password, ...userFields } = values
        const request = new UserRequest()
        request.user = Object.assign(new UserDto(), {
          ...editingUser,
          ...userFields,
          id: editingUser.id
        })
        if (password && password.trim() !== '') {
          request.password = password
        }
        await userClient.updateUserById(request)
        notification.success({
          message: 'Success',
          description: 'User updated',
          placement: 'topRight',
          duration: 2
        })
        userForm.resetFields()
        setIsEditUserModalVisible(false)
        await fetchUsers(userPage)
      }
    } catch {
      notification.error({
        message: 'Error',
        description: 'Failed to edit the user',
        placement: 'topRight',
        duration: 2
      })
    }
  }

  const handleDeleteUser = (userId: number) => {
    Modal.confirm({
      title: 'Delete User',
      content: 'Are you sure you want to delete this user?',
      okText: 'Delete',
      okType: 'danger',
      cancelText: 'Cancel',
      onOk: async () => {
        try {
          await userClient.deleteUserById(userId)
          notification.success({
            message: 'Success',
            description: 'User deleted',
            placement: 'topRight',
            duration: 2
          })
          await fetchUsers(userPage)
        } catch {
          notification.error({
            message: 'Error',
            description: 'Failed to delete user',
            placement: 'topRight',
            duration: 2
          })
        }
      }
    })
  }

  const userColumns: ColumnType<UserDto>[] = [
    { title: 'ID', dataIndex: 'id', key: 'id', width: 80 },
    { title: 'Name', dataIndex: 'name', key: 'name', width: 150 },
    { title: 'Email', dataIndex: 'email', key: 'email', width: 200 },
    {
      key: 'actions',
      align: 'right',
      render: (_: unknown, record: UserDto) =>
        Number(record.userTypeId) === 1 ? null : (
          <>
            <Button
              type="link"
              icon={<EditOutlined />}
              style={{ fontSize: 20 }}
              onClick={() => handleEditUser(record)}
            />
            <Button
              type="link"
              danger
              icon={<DeleteOutlined />}
              style={{ fontSize: 20 }}
              onClick={() => record.id !== undefined && handleDeleteUser(record.id)}
            />
          </>
        ),
      width: 120
    }
  ]

  return (
    <Layout style={{ minHeight: '100vh' }}>
      <Content style={{ margin: 24, background: '#fff', padding: 24 }}>
        <h2>
          <Button
            type="primary"
            icon={<UserAddOutlined />}
            style={{ float: 'right' }}
            onClick={handleAddUser}
          >
            Add User
          </Button>
        </h2>
        <Input.Search
          placeholder="Search users by name"
          value={userSearch}
          onChange={(e) => setUserSearch(e.target.value)}
          onSearch={() => fetchUsers(1)}
          style={{ width: 400, marginBottom: 16 }}
          allowClear
        />
        <Table
          dataSource={users}
          columns={userColumns}
          rowKey="id"
          pagination={{
            current: userPage,
            pageSize: userPageSize,
            total: userTotal,
            showSizeChanger: true,
            pageSizeOptions: ['10', '20', '50', '100'],
            onChange: (page, pageSize) => {
              if (pageSize !== userPageSize) {
                setUserPage(1)
                setUserPageSize(pageSize)
              } else {
                setUserPage(page)
              }
            }
          }}
        />
        <Modal
          title="Add User"
          open={isAddUserModalVisible}
          onCancel={() => setIsAddUserModalVisible(false)}
          onOk={handleAddUserSubmit}
          okText="Add"
        >
          <Form form={addUserForm} layout="vertical">
            <Form.Item
              name="name"
              label="Name"
              rules={[{ required: true, message: 'Name is required' }]}
            >
              <Input />
            </Form.Item>
            <Form.Item
              name="email"
              label="Email"
              rules={[
                { required: true, message: 'Email is required' },
                { type: 'email', message: 'Please enter a valid email address' }
              ]}
            >
              <Input />
            </Form.Item>
            <Form.Item
              name="password"
              label="Password"
              rules={[{ required: true, message: 'Password is required' }]}
            >
              <Input.Password />
            </Form.Item>
            <Form.Item
              name="userTypeId"
              label="User Type"
              rules={[{ required: true, message: 'User type is required' }]}
            >
              <Select placeholder="Select user type">
                {userTypes.map((type) => (
                  <Select.Option key={type.id} value={type.id}>
                    {type.name}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
          </Form>
        </Modal>
        <Modal
          title="Edit User"
          open={isEditUserModalVisible}
          onCancel={() => {
            userForm.resetFields()
            setIsEditUserModalVisible(false)
          }}
          onOk={handleUserEditSubmit}
          okText="Save"
        >
          <Form form={userForm} layout="vertical">
            <Form.Item
              name="name"
              label="Name"
              rules={[{ required: true, message: 'Name is required' }]}
            >
              <Input />
            </Form.Item>
            <Form.Item
              name="email"
              label="Email"
              rules={[
                { required: true, message: 'Email is required' },
                { type: 'email', message: 'Please enter a valid email address' }
              ]}
            >
              <Input />
            </Form.Item>
            <Form.Item name="password" label="Password" rules={[]}>
              <Input.Password placeholder="Leave blank to keep current password" />
            </Form.Item>
            <Form.Item
              name="userTypeId"
              label="User Type"
              rules={[{ required: true, message: 'User type is required' }]}
            >
              <Select placeholder="Select user type">
                {userTypes.map((type) => (
                  <Select.Option key={type.id} value={type.id}>
                    {type.name}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
          </Form>
        </Modal>
      </Content>
    </Layout>
  )
}

export default AdminPage
