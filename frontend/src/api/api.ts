import axios from 'axios';

const api = axios.create({
    baseURL: 'http://localhost:5088', // Replace with your ASP.NET API base URL
    headers: {
        'Content-Type': 'application/json',
    },
});

export default api;