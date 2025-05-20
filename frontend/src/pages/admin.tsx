import React, { useEffect, useMemo, useState } from 'react'
import {
  ContentLibraryClient,
  LibraryDto,
  LibraryFilter,
  UserClient,
  UserDto,
  UserFilter,
  UserRequest,
  UserTypeClient,
  UserTypeDto
} from '../api/apiClient.ts'
import {
  Button,
  Form,
  Input,
  Layout,
  Menu,
  MenuProps,
  message,
  Modal,
  notification,
  Select,
  Table
} from 'antd'
import { DeleteOutlined, EditOutlined, FolderAddOutlined, UserAddOutlined } from '@ant-design/icons'

const { Sider, Content } = Layout

const AdminPage: React.FC = () => {
  const [activeSection, setActiveSection] = useState<'users' | 'libraries'>('users')

  // Users state
  const [users, setUsers] = useState<UserDto[]>([])
  const [userPage, setUserPage] = useState(1)
  const [userTotal, setUserTotal] = useState(0)
  const [userTypes, setUserTypes] = useState<UserTypeDto[]>([])
  const [userSearch, setUserSearch] = useState('')
  const [userPageSize, setUserPageSize] = useState(10)

  // Libraries state
  const [libraries, setLibraries] = useState<LibraryDto[]>([])
  const [libraryPage, setLibraryPage] = useState(1)
  const [libraryTotal, setLibraryTotal] = useState(0)
  const [librarySearch, setLibrarySearch] = useState('')
  const [libraryPageSize, setLibraryPageSize] = useState(10)
  const [allLibraries, setAllLibraries] = useState<LibraryDto[]>([])

  // Add modals
  const [isAddUserModalVisible, setIsAddUserModalVisible] = useState(false)
  const [addUserForm] = Form.useForm()
  const [isAddLibraryModalVisible, setIsAddLibraryModalVisible] = useState(false)
  const [addLibraryForm] = Form.useForm()

  // Edit modals
  const [isEditUserModalVisible, setIsEditUserModalVisible] = useState(false)
  const [editingUser, setEditingUser] = useState<UserDto | null>(null)
  const [userForm] = Form.useForm()
  const [isEditLibraryModalVisible, setIsEditLibraryModalVisible] = useState(false)
  const [editingLibrary, setEditingLibrary] = useState<LibraryDto | null>(null)
  const [libraryForm] = Form.useForm()

  const userClient = new UserClient()
  const contentLibraryClient = new ContentLibraryClient()
  const userTypeClient = useMemo(() => new UserTypeClient(), [])

  useEffect(() => {
    const fetchUserTypes = async () => {
      try {
        const types = await userTypeClient.getAllUserTypes()
        const items = Array.isArray(types) ? types : types?.items || []
        setUserTypes(items)
      } catch {
        message.error('Failed to fetch user types')
      }
    }
    fetchUserTypes()
  }, [userTypeClient])

  useEffect(() => {
    const fetchAllLibraries = async () => {
      try {
        const filter = new LibraryFilter()
        const result = await contentLibraryClient.getAllLibraries(filter)
        const items = Array.isArray(result) ? result : result?.items || []
        setAllLibraries(items)
      } catch {
        // handle error if needed
      }
    }
    fetchAllLibraries()
  }, [])

  useEffect(() => {
    if (activeSection === 'users') fetchUsers(userPage)
    if (activeSection === 'libraries') fetchLibraries(libraryPage)
  }, [activeSection, userPage, libraryPage])

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

  const fetchLibraries = async (page = 1, pageSize = libraryPageSize) => {
    try {
      const filter = new LibraryFilter()
      filter.pageSize = pageSize
      filter.pageNumber = page
      if (librarySearch) filter.libraryName = librarySearch
      const result = await contentLibraryClient.getAllLibraries(filter)
      const items = Array.isArray(result) ? result : result?.items || []
      const total =
        !Array.isArray(result) && typeof result?.totalCount === 'number'
          ? result.totalCount
          : items.length
      setLibraries(items)
      setLibraryTotal(total)
    } catch {
      notification.error({
        message: 'Error',
        description: 'Error fetching libraries',
        placement: 'topRight',
        duration: 2
      })
    }
  }

  useEffect(() => {
    if (activeSection === 'users') {
      fetchUsers(userPage, userPageSize)
    }
  }, [activeSection, userPage, userPageSize])

  useEffect(() => {
    if (activeSection === 'libraries') {
      fetchLibraries(libraryPage, libraryPageSize)
    }
  }, [activeSection, libraryPage, libraryPageSize])

  const handleAddUser = () => {
    addUserForm.resetFields()
    setIsAddUserModalVisible(true)
  }

  const handleAddUserSubmit = async () => {
    try {
      const values = await addUserForm.validateFields()
      console.log('Form values:', values) // Log form values for debugging

      const { password, ...userFields } = values
      const request = new UserRequest()
      request.user = { ...userFields } // Convert to a plain object
      request.password = password

      console.log('Request payload:', request) // Log request payload

      await userClient.createNewUser({
        ...request,
        user: { ...request.user } // Ensure `user` is a plain object
      })
      notification.success({
        message: 'Success',
        description: 'User added',
        placement: 'topRight',
        duration: 2
      })

      setIsAddUserModalVisible(false)
      await fetchUsers(userPage)
    } catch (error) {
      console.error('Error adding user:', error) // Log error details
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

  const handleAddLibrary = () => {
    addLibraryForm.resetFields()
    setIsAddLibraryModalVisible(true)
  }

  const handleAddLibrarySubmit = async () => {
    try {
      const values = await addLibraryForm.validateFields()
      await contentLibraryClient.createNewLibrary(values)
      notification.success({
        message: 'Success',
        description: 'Library added',
        placement: 'topRight',
        duration: 2
      })
      setIsAddLibraryModalVisible(false)
      await fetchLibraries(libraryPage)
    } catch {
      notification.error({
        message: 'Error',
        description: 'Failed to add library',
        placement: 'topRight',
        duration: 2
      })
    }
  }

  const handleEditLibrary = (library: LibraryDto) => {
    setEditingLibrary(library)
    libraryForm.setFieldsValue(library)
    setIsEditLibraryModalVisible(true)
  }

  const handleLibraryEditSubmit = async () => {
    try {
      const values = await libraryForm.validateFields()
      if (editingLibrary) {
        const updatedLibrary = { ...editingLibrary, ...values }
        await contentLibraryClient.updateLibraryById(updatedLibrary)
        notification.success({
          message: 'Success',
          description: 'Library updated',
          placement: 'topRight',
          duration: 2
        })
        setIsEditLibraryModalVisible(false)
        await fetchLibraries(libraryPage)
      }
    } catch {
      notification.error({
        message: 'Error',
        description: 'Failed to edit the library',
        placement: 'topRight',
        duration: 2
      })
    }
  }

  const handleDeleteLibrary = (libraryId: number) => {
    Modal.confirm({
      title: 'Delete Library',
      content: 'Are you sure you want to delete this library?',
      okText: 'Delete',
      okType: 'danger',
      cancelText: 'Cancel',
      onOk: async () => {
        try {
          await contentLibraryClient.deleteLibraryById(libraryId)
          notification.success({
            message: 'Success',
            description: 'Library deleted',
            placement: 'topRight',
            duration: 2
          })
          await fetchLibraries(libraryPage)
        } catch {
          notification.error({
            message: 'Error',
            description: 'Failed to delete library',
            placement: 'topRight',
            duration: 2
          })
        }
      }
    })
  }

  const menuItems: MenuProps['items'] = [
    { key: 'users', label: 'Users' },
    { key: 'libraries', label: 'Libraries' }
  ]

  const userColumns = [
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

  const libraryColumns = [
    { title: 'ID', dataIndex: 'id', key: 'id', width: 80 },
    { title: 'Name', dataIndex: 'name', key: 'name', width: 200 },
    {
      key: 'actions',
      align: 'right',
      render: (_: unknown, record: LibraryDto) => (
        <>
          <Button
            type="link"
            icon={<EditOutlined />}
            style={{ fontSize: 20 }}
            onClick={() => handleEditLibrary(record)}
          />
          <Button
            type="link"
            danger
            icon={<DeleteOutlined />}
            style={{ fontSize: 20 }}
            onClick={() => record.id !== undefined && handleDeleteLibrary(record.id)}
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
          onClick={({ key }) => setActiveSection(key as 'users' | 'libraries')}
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
                      {
                        required: true,
                        message: 'Email is required'
                      },
                      { type: 'email', message: 'Please enter a valid email address' }
                    ]}
                  >
                    <Input />
                  </Form.Item>
                  <Form.Item
                    name="password"
                    label="Password"
                    rules={[
                      {
                        required: true,
                        message: 'Password is required'
                      }
                    ]}
                  >
                    <Input.Password />
                  </Form.Item>
                  <Form.Item
                    name="userTypeId"
                    label="User Type"
                    rules={[
                      {
                        required: true,
                        message: 'User type is required'
                      }
                    ]}
                  >
                    <Select placeholder="Select user type">
                      {userTypes.map((type) => (
                        <Select.Option key={type.id} value={type.id}>
                          {type.name}
                        </Select.Option>
                      ))}
                    </Select>
                  </Form.Item>
                  <Form.Item name="accessibleLibraryIds" label="Accessible Libraries">
                    <Select
                      mode="multiple"
                      placeholder="Select accessible libraries"
                      optionFilterProp="children"
                    >
                      {allLibraries.map((lib) => (
                        <Select.Option key={lib.id} value={lib.id}>
                          {lib.name}
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
                      {
                        required: true,
                        message: 'Email is required'
                      },
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
                    rules={[
                      {
                        required: true,
                        message: 'User type is required'
                      }
                    ]}
                  >
                    <Select placeholder="Select user type">
                      {userTypes.map((type) => (
                        <Select.Option key={type.id} value={type.id}>
                          {type.name}
                        </Select.Option>
                      ))}
                    </Select>
                  </Form.Item>
                  <Form.Item name="accessibleLibraryIds" label="Accessible Libraries">
                    <Select
                      mode="multiple"
                      placeholder="Select accessible libraries"
                      optionFilterProp="children"
                    >
                      {allLibraries.map((lib) => (
                        <Select.Option key={lib.id} value={lib.id}>
                          {lib.name}
                        </Select.Option>
                      ))}
                    </Select>
                  </Form.Item>
                </Form>
              </Modal>
            </>
          )}
          {activeSection === 'libraries' && (
            <>
              <h2>
                <Button
                  type="primary"
                  icon={<FolderAddOutlined />}
                  style={{ float: 'right' }}
                  onClick={handleAddLibrary}
                >
                  Add Library
                </Button>
              </h2>
              <Input.Search
                placeholder="Search libraries by name"
                value={librarySearch}
                onChange={(e) => setLibrarySearch(e.target.value)}
                onSearch={() => fetchLibraries(1)}
                style={{ width: 400, marginBottom: 16 }}
                allowClear
              />
              <Table
                dataSource={libraries}
                columns={libraryColumns}
                rowKey="id"
                pagination={{
                  current: libraryPage,
                  pageSize: libraryPageSize,
                  total: libraryTotal,
                  showSizeChanger: true,
                  pageSizeOptions: ['10', '20', '50', '100'],
                  onChange: (page, pageSize) => {
                    if (pageSize !== libraryPageSize) {
                      setLibraryPage(1)
                      setLibraryPageSize(pageSize)
                    } else {
                      setLibraryPage(page)
                    }
                  }
                }}
              />
              <Modal
                title="Add Library"
                open={isAddLibraryModalVisible}
                onCancel={() => setIsAddLibraryModalVisible(false)}
                onOk={handleAddLibrarySubmit}
                okText="Add"
              >
                <Form form={addLibraryForm} layout="vertical">
                  <Form.Item
                    name="name"
                    label="Name"
                    rules={[{ required: true, message: 'Name is required' }]}
                  >
                    <Input />
                  </Form.Item>
                </Form>
              </Modal>
              <Modal
                title="Edit Library"
                open={isEditLibraryModalVisible}
                onCancel={() => setIsEditLibraryModalVisible(false)}
                onOk={handleLibraryEditSubmit}
                okText="Save"
              >
                <Form form={libraryForm} layout="vertical">
                  <Form.Item
                    name="name"
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
