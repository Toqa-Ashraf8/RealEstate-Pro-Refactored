import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { variables } from '../../../assets/variables.js'; 
import { updateSelectedProjectCode } from '../../../assets/redux/projectSlice.js'; 
import './ProjectsReview.css'; 
import { 
  FaMapMarkerAlt, 
  FaBuilding, 
  FaHome, 
  FaStore, 
  FaHospital, 
  FaImage, 
  FaChevronLeft,
  FaExternalLinkAlt
} from 'react-icons/fa';
import { useNavigate } from 'react-router-dom';
import { fetchProjectsList } from '../../../services/projectService.js';

const ProjectsReview = () => {
  const projectState = useSelector((state) => state.projects);
  const dispatch = useDispatch();
  const navigate = useNavigate();

  useEffect(() => {
    dispatch(fetchProjectsList());
  }, [dispatch]);

  const getProjectIcon = (type) => {
    switch(type) {
      case 'سكني': return <FaHome className="type-icon residential" />;
      case 'تجاري': return <FaStore className="type-icon commercial" />;
      case 'إداري': return <FaBuilding className="type-icon office" />;
      case 'طبي': return <FaHospital className="type-icon medical" />;
      default: return <FaBuilding className="type-icon" />;
    }
  };

  const handleRowClick = (index) => {
    dispatch(updateSelectedProjectCode(index));
    navigate('/units');
  };

  return (
    <div className="table-page-wrapper" dir="rtl">
      <div className="table-header-section">
        <h2>إدارة المشاريع</h2>
        <p>عرض وتحليل كافة المشاريع القائمة في النظام</p>
      </div>

      <div className="table-container animate__animated animate__fadeIn">
        <table className="modern-projects-table">
          <thead>
            <tr>
              <th>المشروع</th>
              <th>النوع</th>
              <th>الموقع</th>
              <th>عدد الوحدات</th>
              <th>الحالة</th>
              <th>الإجراءات</th>
            </tr>
          </thead>
          <tbody>
            {projectState.projectsList.length === 0 ? (
              <tr>
                <td colSpan="6" className="empty-state">
                  <FaImage size={30} />
                  <p>لا توجد مشاريع متاحة حالياً</p>
                </td>
              </tr>
            ) : (
              projectState.projectsList.map((project, i) => (
                <tr key={i} onClick={() => handleRowClick(i)}>
                  <td className="project-info-cell">
                    <div className="project-img-mini">
                      {project.ProjectImage ? (
                        <img src={variables.PROJECT_IMAGES_URL + project.ProjectImage} alt="" />
                      ) : (
                        <div className="no-img-small"><FaImage /></div>
                      )}
                    </div>
                    <div className="project-name-wrapper">
                      <span className="project-main-name">{project.ProjectName}</span>
                      <span className="project-id">#PRJ-{100 + i}</span>
                    </div>
                  </td>
                  <td>
                    <div className="type-badge-wrapper">
                      {getProjectIcon(project.ProjectType)}
                      <span>{project.ProjectType}</span>
                    </div>
                  </td>
                  <td>
                    <div className="location-cell">
                      <FaMapMarkerAlt />
                      <span>{project.Location || 'غير محدد'}</span>
                    </div>
                  </td>
                  <td>
                    <div className="units-count">
                      <strong>{project.TotalUnits || 0}</strong> وحدة
                    </div>
                  </td>
                  <td>
                    <span className={`status-pill ${project.ProjectStatus === 'نشط' ? 'active' : 'pending'}`}>
                      {project.ProjectStatus || 'نشط'}
                    </span>
                  </td>
                  <td className="actions-cell">
                    <button className="view-details-btn">
                      التفاصيل <FaChevronLeft />
                    </button>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default ProjectsReview;