import React, { useState, useEffect, useMemo} from 'react'
import {
  UserClient,
  UploadClient,
  ContentLibraryClient,
  UserTypeClient,
  UserDto,
  CreateUserRequest,
  UploadDto,
  LibraryDto,
  UserTypeDto,
  UserFilter,
  UploadFilter,
  LibraryFilter,
} from '../api/apiClient.ts'
import { Layout, Menu, Table, Button, Modal, message, notification, Form, Input, Select } from 'antd'
import type { MenuProps } from 'antd'
import { PlusOutlined } from '@ant-design/icons'

const { Sider, Content } = Layout

const PAGE_SIZE = 10

const AdminPage: React.FC = () => {
  const [activeSection, setActiveSection] = useState<'users' | 'libraries' | 'uploads'>('users');

  // Users state
  const [users, setUsers] = useState<UserDto[]>([]);
  const [userPage, setUserPage] = useState(1);
  const [userTotal, setUserTotal] = useState(0);
  const [userTypes, setUserTypes] = useState<UserTypeDto[]>([]);
  
  // Libraries state
  const [libraries, setLibraries] = useState<LibraryDto[]>([]);
  const [libraryPage, setLibraryPage] = useState(1);
  const [libraryTotal, setLibraryTotal] = useState(0);

  // Uploads state
  const [uploads, setUploads] = useState<UploadDto[]>([]);
  const [uploadPage, setUploadPage] = useState(1);
  const [uploadTotal, setUploadTotal] = useState(0);

  // Add modals
  const [isAddUserModalVisible, setIsAddUserModalVisible] = useState(false);
  const [addUserForm] = Form.useForm();
  const [isAddLibraryModalVisible, setIsAddLibraryModalVisible] = useState(false);
  const [addLibraryForm] = Form.useForm();

  // Edit modals
  const [isEditUserModalVisible, setIsEditUserModalVisible] = useState(false);
  const [editingUser, setEditingUser] = useState<UserDto | null>(null);
  const [userForm] = Form.useForm();
  const [isEditLibraryModalVisible, setIsEditLibraryModalVisible] = useState(false);
  const [editingLibrary, setEditingLibrary] = useState<LibraryDto | null>(null);
  const [libraryForm] = Form.useForm();

  const userClient = new UserClient();
  const uploadClient = new UploadClient();
  const contentLibraryClient = new ContentLibraryClient();
  const userTypeClient = useMemo(() => new UserTypeClient(), []);

  useEffect(() => {
    const fetchUserTypes = async () => {
      try {
        const types = await userTypeClient.getAllUserTypes();
        const items = Array.isArray(types) ? types : types?.items || [];
        setUserTypes(items);
      } catch {
        message.error('Failed to fetch user types');
      }
    };
    fetchUserTypes();
  }, [userTypeClient]);
  
  useEffect(() => {
    if (activeSection === 'users') fetchUsers(userPage);
    if (activeSection === 'libraries') fetchLibraries(libraryPage);
    if (activeSection === 'uploads') fetchUploads(uploadPage);
  }, [activeSection, userPage, libraryPage, uploadPage]);
  
  const fetchUsers = async (page = 1) => {
    try {
      const filter = new UserFilter();
      filter.pageSize = PAGE_SIZE;
      filter.pageNumber = page;
      const result = await userClient.getAllUsers(filter);
      const items = Array.isArray(result) ? result : result?.items || [];
      const total = !Array.isArray(result) && typeof result?.totalCount === 'number' ? result.totalCount : items.length;
      setUsers(items);
      setUserTotal(total);
    } catch {
      notification.error({
        message: 'Error',
        description: 'Error fetching users',
        placement: 'topRight',
        duration: 2
      });
    }
  };

  const fetchLibraries = async (page = 1) => {
    try {
      const filter = new LibraryFilter();
      filter.pageSize = PAGE_SIZE;
      filter.pageNumber = page;
      const result = await contentLibraryClient.getAllLibraries(filter);
      const items = Array.isArray(result) ? result : result?.items || [];
      const total = !Array.isArray(result) && typeof result?.totalCount === 'number' ? result.totalCount : items.length;
      setLibraries(items);
      setLibraryTotal(total);
    } catch {
      notification.error({
        message: 'Error',
        description: 'Error fetching libraries',
        placement: 'topRight',
        duration: 2
      });
    }
  };

  const fetchUploads = async (page = 1) => {
    try {
      const filter = new UploadFilter();
      filter.pageSize = PAGE_SIZE;
      filter.pageNumber = page;
      const result = await uploadClient.getAllUploads(filter);
      const items = Array.isArray(result) ? result : result?.items || [];
      const total = !Array.isArray(result) && typeof result?.totalCount === 'number' ? result.totalCount : items.length;
      setUploads(items);
      setUploadTotal(total);
    } catch {
      notification.error({
        message: 'Error',
        description: 'Error fetching uploads',
        placement: 'topRight',
        duration: 2
      });
    }
  };

  const handleAddUser = () => {
    addUserForm.resetFields();
    setIsAddUserModalVisible(true);
  };

  const handleAddUserSubmit = async () => {
    try {
      const values = await addUserForm.validateFields();
      await userClient.createNewUser(values);
      notification.success({
        message: 'Success',
        description: 'User added',
        placement: 'topRight',
        duration: 2
      });
      setIsAddUserModalVisible(false);
      await fetchUsers(userPage);
    } catch {
      notification.error({
        message: 'Error',
        description: 'Failed to add user',
        placement: 'topRight',
        duration: 2
      });
    }
  };

  const handleEditUser = (user: UserDto) => {
    setEditingUser(user);
    userForm.setFieldsValue(user);
    setIsEditUserModalVisible(true);
  };
  
  const handleUserEditSubmit = async () => {
    try {
      const values = await userForm.validateFields();
      if (editingUser) {
        const updatedUser = { ...editingUser, ...values };
        await userClient.updateUserById(updatedUser);
        notification.success({
          message: 'Success',
          description: 'User updated',
          placement: 'topRight',
          duration: 2
        });
        setIsEditUserModalVisible(false);
        await fetchUsers(userPage);
      }
    } catch {
      notification.error({
        message: 'Error',
        description: 'Failed to edit the user',
        placement: 'topRight',
        duration: 2
      });
    }
  };

  const handleDeleteUser = (userId: number) => {
    Modal.confirm({
      title: 'Delete User',
      content: 'Are you sure you want to delete this user?',
      okText: 'Delete',
      okType: 'danger',
      cancelText: 'Cancel',
      onOk: async () => {
        try {
          await userClient.deleteUserById(userId);
          notification.success({
            message: 'Success',
            description: 'User deleted',
            placement: 'topRight',
            duration: 2
          });
          await fetchUsers(userPage);
        } catch {
          notification.error({
            message: 'Error',
            description: 'Failed to delete user',
            placement: 'topRight',
            duration: 2
          });
        }
      }
    });
  };
  
  const handleAddLibrary = () => {
    addLibraryForm.resetFields();
    setIsAddLibraryModalVisible(true);
  };

  const handleAddLibrarySubmit = async () => {
    try {
      const values = await addLibraryForm.validateFields();
      await contentLibraryClient.createNewLibrary(values);
      notification.success({
        message: 'Success',
        description: 'Library added',
        placement: 'topRight',
        duration: 2
      });
      setIsAddLibraryModalVisible(false);
      await fetchLibraries(libraryPage);
    } catch {
      notification.error({
        message: 'Error',
        description: 'Failed to add library',
        placement: 'topRight',
        duration: 2
      });
    }
  };

  const handleEditLibrary = (library: LibraryDto) => {
    setEditingLibrary(library);
    libraryForm.setFieldsValue(library);
    setIsEditLibraryModalVisible(true);
  };

  const handleLibraryEditSubmit = async () => {
    try {
      const values = await libraryForm.validateFields();
      if (editingLibrary) {
        const updatedLibrary = { ...editingLibrary, ...values };
        await contentLibraryClient.updateLibraryById(updatedLibrary);
        notification.success({
          message: 'Success',
          description: 'Library updated',
          placement: 'topRight',
          duration: 2
        });
        setIsEditLibraryModalVisible(false);
        await fetchLibraries(libraryPage);
      }
    } catch {
      notification.error({
        message: 'Error',
        description: 'Failed to edit the library',
        placement: 'topRight',
        duration: 2
      });
    }
  };

  const handleDeleteLibrary = (libraryId: number) => {
    Modal.confirm({
      title: 'Delete Library',
      content: 'Are you sure you want to delete this library?',
      okText: 'Delete',
      okType: 'danger',
      cancelText: 'Cancel',
      onOk: async () => {
        try {
          await contentLibraryClient.deleteLibraryById(libraryId);
          notification.success({
            message: 'Success',
            description: 'Library deleted',
            placement: 'topRight',
            duration: 2
          });
          await fetchLibraries(libraryPage);
        } catch {
          notification.error({
            message: 'Error',
            description: 'Failed to delete library',
            placement: 'topRight',
            duration: 2
          });
        }
      }
    });
  };
  
  const handleViewUpload = (upload: UploadDto) => {
    message.info('View Upload not implemented');
  };

  const handleDeleteUpload = (uploadId: number) => {
    Modal.confirm({
      title: 'Delete Upload',
      content: 'Are you sure you want to delete this upload?',
      okText: 'Delete',
      okType: 'danger',
      cancelText: 'Cancel',
      onOk: async () => {
        try {
          await uploadClient.deleteUploadById(uploadId);
          message.success('Upload deleted');
          await fetchUploads(uploadPage);
        } catch {
          message.error('Failed to delete upload');
        }
      }
    });
  };

  const menuItems: MenuProps['items'] = [
    { key: 'users', label: 'Users' },
    { key: 'libraries', label: 'Libraries' },
    { key: 'uploads', label: 'Uploads' }
  ];

  const userColumns = [
    { title: 'ID', dataIndex: 'id', key: 'id', width: 80 },
    { title: 'Name', dataIndex: 'name', key: 'name', width: 150 },
    { title: 'Email', dataIndex: 'email', key: 'email', width: 200 },
    {
      title: 'Actions',
      key: 'actions',
      render: (_: unknown, record: UserDto) => (
        record.userTypeId === 1 ? null : (
          <>
            <Button type="link" onClick={() => handleEditUser(record)}>Edit</Button>
            <Button type="link" danger onClick={() => record.id !== undefined && handleDeleteUser(record.id)}>Delete</Button>
          </>
        )
      ),
      width: 120
    }
  ];

  const libraryColumns = [
    { title: 'ID', dataIndex: 'id', key: 'id', width: 80 },
    { title: 'Name', dataIndex: 'name', key: 'name', width: 200 },
    {
      title: 'Actions',
      key: 'actions',
      render: (_: unknown, record: LibraryDto) => (
        <>
          <Button type="link" onClick={() => handleEditLibrary(record)}>Edit</Button>
          <Button type="link" danger onClick={() => record.id !== undefined && handleDeleteLibrary(record.id)}>Delete</Button>
        </>
      ),
      width: 120
    }
  ];

  const uploadColumns = [
    { title: 'ID', dataIndex: 'id', key: 'id', width: 80 },
    { title: 'Owner ID', dataIndex: 'ownerId', key: 'ownerId', width: 100 },
    {
      title: 'Date',
      dataIndex: 'date',
      key: 'date',
      width: 120,
      render: (date: Date) => date ? new Date(date).toLocaleDateString() : ''
    },
    { title: 'Type', dataIndex: 'type', key: 'type', width: 120 },
    {
      title: 'Actions',
      key: 'actions',
      render: (_: unknown, record: UploadDto) => (
        <>
          <Button type="link" onClick={() => handleViewUpload(record)}>View</Button>
          <Button type="link" danger onClick={() => record.id !== undefined && handleDeleteUpload(record.id)}>Delete</Button>        </>
      ),
      width: 120
    }
  ];

  return (
    <Layout style={{ minHeight: '100vh' }}>
      <Sider width={220} style={{ background: '#fff' }}>
        <div style={{ padding: 24, fontWeight: 'bold', fontSize: 20 }}>Admin Dashboard</div>
        <Menu
          mode="inline"
          selectedKeys={[activeSection]}
          items={menuItems}
          onClick={({ key }) => setActiveSection(key as 'users' | 'libraries' | 'uploads')}
        />
      </Sider>
      <Layout>
        <Content style={{ margin: 24, background: '#fff', padding: 24 }}>
          {activeSection === 'users' && (
            <>
              <h2>
                Users Management
                <Button
                  type="primary"
                  icon={<PlusOutlined />}
                  style={{ float: 'right' }}
                  onClick={handleAddUser}
                >
                  Add User
                </Button>
              </h2>
              <Table
                dataSource={users}
                columns={userColumns}
                rowKey="id"
                pagination={{
                  current: userPage,
                  pageSize: PAGE_SIZE,
                  total: userTotal,
                  onChange: (page) => setUserPage(page),
                }}
              />
              {/* Add User Modal */}
              <Modal
                title="Add User"
                open={isAddUserModalVisible}
                onCancel={() => setIsAddUserModalVisible(false)}
                onOk={handleAddUserSubmit}
                okText="Add"
              >
                <Form form={addUserForm} layout="vertical">
                  <Form.Item name="name" label="Name" rules={[{ required: true, message: 'Name is required' }]}>
                    <Input />
                  </Form.Item>
                  <Form.Item name="email" label="Email" rules={[{ required: true, message: 'Email is required' }, { type: 'email', message: 'Please enter a valid email address' }]}>
                    <Input />
                  </Form.Item>
                  <Form.Item name="password" label="Password" rules={[{ required: true, message: 'Password is required' }]}>
                    <Input.Password />
                  </Form.Item>
                  <Form.Item name="userTypeId" label="User Type" rules={[{ required: true, message: 'User type is required' }]}>
                    <Select placeholder="Select user type">
                      {userTypes.map(type => (
                        <Select.Option key={type.id} value={type.id}>
                          {type.name}
                        </Select.Option>
                      ))}
                    </Select>
                  </Form.Item>
                  <Form.Item
                    name="accessibleLibraryIds"
                    label="Accessible Libraries"
                  >
                    <Select
                      mode="multiple"
                      placeholder="Select accessible libraries"
                      optionFilterProp="children"
                    >
                      {libraries.map(lib => (
                        <Select.Option key={lib.id} value={lib.id}>
                          {lib.name}
                        </Select.Option>
                      ))}
                    </Select>
                  </Form.Item>
                </Form>
              </Modal>
              {/* Edit User Modal */}
              <Modal
                title="Edit User"
                open={isEditUserModalVisible}
                onCancel={() => setIsEditUserModalVisible(false)}
                onOk={handleUserEditSubmit}
                okText="Save"
              >
                <Form form={userForm} layout="vertical">
                  <Form.Item name="name" label="Name" rules={[{ required: true, message: 'Name is required' }]}>
                    <Input />
                  </Form.Item>
                  <Form.Item name="email" label="Email" rules={[{ required: true, message: 'Email is required' }, { type: 'email', message: 'Please enter a valid email address' }]}>
                    <Input />
                  </Form.Item>
                  <Form.Item name="userTypeId" label="User Type" rules={[{ required: true, message: 'User type is required' }]}>
                    <Select placeholder="Select user type">
                      {userTypes.map(type => (
                        <Select.Option key={type.id} value={type.id}>
                          {type.name}
                        </Select.Option>
                      ))}
                    </Select>
                  </Form.Item>
                  <Form.Item
                    name="accessibleLibraryIds"
                    label="Accessible Libraries"
                  >
                    <Select
                      mode="multiple"
                      placeholder="Select accessible libraries"
                      optionFilterProp="children"
                    >
                      {libraries.map(lib => (
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
                Libraries Management
                <Button
                  type="primary"
                  icon={<PlusOutlined />}
                  style={{ float: 'right' }}
                  onClick={handleAddLibrary}
                >
                  Add Library
                </Button>
              </h2>
              <Table
                dataSource={libraries}
                columns={libraryColumns}
                rowKey="id"
                pagination={{
                  current: libraryPage,
                  pageSize: PAGE_SIZE,
                  total: libraryTotal,
                  onChange: (page) => setLibraryPage(page),
                }}
              />
              {/* Add Library Modal */}
              <Modal
                title="Add Library"
                open={isAddLibraryModalVisible}
                onCancel={() => setIsAddLibraryModalVisible(false)}
                onOk={handleAddLibrarySubmit}
                okText="Add"
              >
                <Form form={addLibraryForm} layout="vertical">
                  <Form.Item name="name" label="Name" rules={[{ required: true , message: 'Name is required' }]}>
                    <Input />
                  </Form.Item>
                </Form>
              </Modal>
              {/* Edit Library Modal */}
              <Modal
                title="Edit Library"
                open={isEditLibraryModalVisible}
                onCancel={() => setIsEditLibraryModalVisible(false)}
                onOk={handleLibraryEditSubmit}
                okText="Save"
              >
                <Form form={libraryForm} layout="vertical">
                  <Form.Item name="name" label="Name" rules={[{ required: true }]}>
                    <Input />
                  </Form.Item>
                </Form>
              </Modal>
            </>
          )}
          {activeSection === 'uploads' && (
            <>
              <h2>Uploads Management</h2>
              <Table
                dataSource={uploads}
                columns={uploadColumns}
                rowKey="id"
                pagination={{
                  current: uploadPage,
                  pageSize: PAGE_SIZE,
                  total: uploadTotal,
                  onChange: (page) => setUploadPage(page),
                }}
              />
            </>
          )}
        </Content>
      </Layout>
    </Layout>
  );
};

export default AdminPage;