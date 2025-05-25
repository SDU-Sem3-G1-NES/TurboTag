import React, { useState, useEffect, useMemo } from 'react'
import {
  UserClient,
  TagClient,
  UserTypeClient,
  UserDto,
  TagDto,
  UserRequest,
  UserTypeDto,
  UserFilter,
  TagFilter,
} from '../api/apiClient.ts'
import {
  Layout,
  Menu,
  Table,
  Button,
  Modal,
  notification,
  Form,
  Input,
  Select,
  MenuProps
} from 'antd'
import { UserAddOutlined, FolderAddOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons'

const { Sider, Content } = Layout

const AdminPage: React.FC = () => {
  const [activeSection, setActiveSection] = useState<'users' | 'tags'>('users')

  // Users state
  const [users, setUsers] = useState<UserDto[]>([])
  const [userPage, setUserPage] = useState(1)
  const [userTotal, setUserTotal] = useState(0)
  const [userTypes, setUserTypes] = useState<UserTypeDto[]>([])
  const [userSearch, setUserSearch] = useState('')
  const [userPageSize, setUserPageSize] = useState(10)

  // Tags state
  const [tags, setTags] = useState<TagDto[]>([])
  const [tagPage, setTagPage] = useState(1)
  const [tagTotal, setTagTotal] = useState(0)
  const [tagSearch, setTagSearch] = useState('')
  const [tagPageSize, setTagPageSize] = useState(10)

  // Add modals
  const [isAddUserModalVisible, setIsAddUserModalVisible] = useState(false)
  const [addUserForm] = Form.useForm()
  const [isAddTagModalVisible, setIsAddTagModalVisible] = useState(false)
  const [addTagForm] = Form.useForm()

  // Edit modals
  const [isEditUserModalVisible, setIsEditUserModalVisible] = useState(false)
  const [editingUser, setEditingUser] = useState<UserDto | null>(null)
  const [userForm] = Form.useForm()

  const userClient = new UserClient()
  const tagClient = new TagClient()
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
    if (activeSection === 'users') {
      fetchUsers(userPage, userPageSize)
    }
  }, [activeSection, userPage, userPageSize])

  useEffect(() => {
    if (activeSection === 'tags') {
      fetchTags(tagPage, tagPageSize)
    }
  }, [activeSection, tagPage, tagPageSize])

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

  const fetchTags = async (page = 1, pageSize = tagPageSize) => {
    try {
      const filter = new TagFilter()
      filter.pageSize = pageSize
      filter.pageNumber = page
      if (tagSearch) filter.names = [tagSearch]
      const result = await tagClient.getAllTags(filter)
      const items = Array.isArray(result) ? result : result?.items || []
      const total =
        !Array.isArray(result) && typeof result?.totalCount === 'number'
          ? result.totalCount
          : items.length
      setTags(items)
      setTagTotal(total)
    } catch {
      notification.error({
        message: 'Error',
        description: 'Error fetching tags',
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

  const handleAddTag = () => {
    addTagForm.resetFields()
    setIsAddTagModalVisible(true)
  }

  const handleAddTagSubmit = async () => {
    try {
      const values = await addTagForm.validateFields()
      await tagClient.addTag(values)
      notification.success({
        message: 'Success',
        description: 'Tag added',
        placement: 'topRight',
        duration: 2
      })
      setIsAddTagModalVisible(false)
      await fetchTags(tagPage)
    } catch {
      notification.error({
        message: 'Error',
        description: 'Failed to add tag',
        placement: 'topRight',
        duration: 2
      })
    }
  }

  const handleDeleteTag = (tagId: number) => {
    Modal.confirm({
      title: 'Delete Tag',
      content: 'Are you sure you want to delete this tag?',
      okText: 'Delete',
      okType: 'danger',
      cancelText: 'Cancel',
      onOk: async () => {
        try {
          const filter = new TagFilter()
          filter.ids = [tagId]
          await tagClient.deleteTagById(filter)
          notification.success({
            message: 'Success',
            description: 'Tag deleted',
            placement: 'topRight',
            duration: 2
          })
          await fetchTags(tagPage)
        } catch {
          notification.error({
            message: 'Error',
            description: 'Failed to delete tag',
            placement: 'topRight',
            duration: 2
          })
        }
      }
    })
  }

  const menuItems: MenuProps['items'] = [
    { key: 'users', label: 'Users' },
    { key: 'tags', label: 'Tags' }
  ]

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

  const tagColumns: ColumnType<TagDto>[] = [
    { title: 'ID', dataIndex: 'tagId', key: 'id', width: 80 },
    { title: 'Name', dataIndex: 'tagName', key: 'name', width: 200 },
    {
      key: 'actions',
      align: 'right',
      render: (_: unknown, record: TagDto) => (
        <>
          <Button
            type="link"
            danger
            icon={<DeleteOutlined />}
            style={{ fontSize: 20 }}
            onClick={() => record.tagId !== undefined && handleDeleteTag(record.tagId)}
          />
        </>
      ),
      width: 120
    }
  ]

  return (
    <Layout style={{ minHeight: '100vh' }}>
      <Sider width={220} style={{ background: '#fff' }}>
        <div style={{ padding: 24, fontWeight: 'bold', fontSize: 20 }}>Admin Dashboard</div>
        <Menu
          mode="inline"
          selectedKeys={[activeSection]}
          items={menuItems}
          onClick={({ key }) => setActiveSection(key as 'users' | 'tags')}
        />
      </Sider>
      <Layout>
        <Content style={{ margin: 24, background: '#fff', padding: 24 }}>
          {activeSection === 'users' && (
            <>
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
            </>
          )}
          {activeSection === 'tags' && (
            <>
              <h2>
                <Button
                  type="primary"
                  icon={<FolderAddOutlined />}
                  style={{ float: 'right' }}
                  onClick={handleAddTag}
                >
                  Add Tag
                </Button>
              </h2>
              <Input.Search
                placeholder="Search tags by name"
                value={tagSearch}
                onChange={(e) => setTagSearch(e.target.value)}
                onSearch={() => fetchTags(1)}
                style={{ width: 400, marginBottom: 16 }}
                allowClear
              />
              <Table
                dataSource={tags}
                columns={tagColumns}
                rowKey="id"
                pagination={{
                  current: tagPage,
                  pageSize: tagPageSize,
                  total: tagTotal,
                  showSizeChanger: true,
                  pageSizeOptions: ['10', '20', '50', '100'],
                  onChange: (page, pageSize) => {
                    if (pageSize !== tagPageSize) {
                      setTagPage(1)
                      setTagPageSize(pageSize)
                    } else {
                      setTagPage(page)
                    }
                  }
                }}
              />
              <Modal
                title="Add Tag"
                open={isAddTagModalVisible}
                onCancel={() => setIsAddTagModalVisible(false)}
                onOk={handleAddTagSubmit}
                okText="Add"
              >
                <Form form={addTagForm} layout="vertical">
                  <Form.Item
                    name="TagName"
                    label="Name"
                    rules={[{ required: true, message: 'Name is required' }]}
                  >
                    <Input />
                  </Form.Item>
                </Form>
              </Modal>
            </>
          )}
        </Content>
      </Layout>
    </Layout>
  )
}

export default AdminPage