import { Navigate } from 'react-router-dom';
import { useSelector } from 'react-redux';
import { toast } from 'react-toastify';

const ProtectedRoute = ({ children, allowedRoles }) => {
    const { token, userDetails } = useSelector((state) => state.auth);

    if (!token) {
        return <Navigate to="/login" replace />;
    }

     if (allowedRoles && !allowedRoles.includes(userDetails.Role)) { 
        return <Navigate to="/projects" replace />;
    } 
    return children;
};

export default ProtectedRoute;