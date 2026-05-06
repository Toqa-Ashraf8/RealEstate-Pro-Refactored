import React from 'react';
import { MdClose, MdWarning, MdDelete, MdOutlineCancel } from "react-icons/md";
import './ProjectDeleteModal.css';
import { useDispatch, useSelector } from 'react-redux';
import { resetProjectForm, toggleDeleteProjectModal } from '../../../assets/redux/projectSlice.js';
import { toast } from 'react-toastify';
import { deleteProject } from '../../../services/projectService.js';

const ProjectDeleteModal = () => {
  const { project } = useSelector((state) => state.projects);
  const { isLoading } = useSelector((state) => state.ui);
  const dispatch = useDispatch();

  const deleteForm = async () => {
    if(project.ProjectCode >0 ){
      await dispatch(deleteProject(project.ProjectCode));
      await dispatch(toggleDeleteProjectModal(false));
      await dispatch(resetProjectForm());
      toast.error("تم حذف بيانات المشروع بنجاح", {
        theme: "colored",
        position: "top-right"
      });
    }
  }

  return (
    <div className="delete-wrapper">
      <div className="delete-card">
        <button 
          className="delete-close-btn" 
          disabled={isLoading}
          onClick={() => dispatch(toggleDeleteProjectModal(false))}
          title="إغلاق"
        >
          <MdClose />
        </button>

        <div className="delete-content">
          <div className="delete-icon-box">
            <MdWarning />
          </div>
          <h2 className="delete-title">تأكيد الحذف</h2>
          <div className="delete-description">
            هل أنت متأكد من حذف جميع بيانات هذا المشروع؟ 
         
          </div>
        </div>

        <div className="delete-actions">
           <button 
            className="btn-action btn-yes-p" 
            disabled={isLoading}
            onClick={() => deleteForm()}
          >
            {isLoading ? "جاري الحذف..." : ' متأكد'}
          </button>
          
          <button 
            className="btn-action btn-no" 
            disabled={isLoading}
            onClick={() => dispatch(toggleDeleteProjectModal(false))}
          >
              إلغاء
          </button>
        </div>
      </div>
    </div>
  );
};

export default ProjectDeleteModal;