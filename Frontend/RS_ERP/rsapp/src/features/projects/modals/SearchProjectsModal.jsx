import React, { useEffect } from 'react'
import './SearchProjectsModal.css'
import { useDispatch, useSelector } from 'react-redux';
import { 
  selectProjectFromSearch, 
  toggleSearchModal 
} from '../../../assets/redux/projectSlice.js';
import { variables } from '../../../assets/variables.js';
import { 
  fetchProjectsList,
  fetchUnitsDetails 
} from '../../../services/projectService.js';

const SearchProjectsModal = () => {
  const {
    selectedProjectRowIndex,
    projectsList
  } = useSelector((state) => state.projects);
  const dispatch = useDispatch();

 useEffect(() => {
   const fetchProjectsData =async  () => { 
    await dispatch(fetchProjectsList());
  }; 
  fetchProjectsData(); 
}, []);

const fillClientForm=async(i)=>{
     await dispatch(selectProjectFromSearch(i));
}
useEffect(() => {
  if (selectedProjectRowIndex) {
    dispatch(fetchUnitsDetails(selectedProjectRowIndex));
    dispatch(toggleSearchModal(false));
  }
}, [selectedProjectRowIndex]);

  return (
    <div dir='rtl'>
      <div className="modals">
        <div className="modalcnt_s">
          <div className="hdr_s">
            <div className='hrdtitles'>
              <span 
                style={{
                  color: '#fff',
                  cursor: 'pointer',
                  fontSize: '32px',
                  lineHeight: '1'
                }}
                onClick={() => dispatch(toggleSearchModal(false))}
              >&times;</span>
            </div> 
            <h3 className='hds_title'>استعراض المشاريع</h3>
          </div>

          <div className="bodys">
            <div className='tbl_ss'>
              <table className='table table-hover tbl-srch'>
                <thead>
                  <tr>
                    <th>كود المشروع</th>
                    <th>المشروع</th>
                    <th>النوع</th>
                    <th>الموقع</th>
                    <th>عدد الوحدات</th>
                    <th>الحالة</th>
                    <th>الصورة</th>
                  </tr>
                </thead>
                <tbody>
                  {projectsList.length === 0 ? (
                    <tr>
                      <td colSpan={7} className="empty-msg" style={{padding: '40px'}}>لا توجد بيانات لعرضها</td>
                    </tr>
                  ) : (
                    projectsList.map((project, i) => (
                      <tr key={i} onClick={() => fillClientForm(i)}>
                        <td>{project.ProjectCode}</td>
                        <td style={{fontWeight: '700'}}>{project.ProjectName}</td>
                        <td>{project.ProjectType}</td>
                        <td>{project.Location}</td>
                        <td>{project.TotalUnits}</td>
                        <td>
                          <span className="badge bg-light text-dark" 
                          style={{padding: '8px 12px', borderRadius: '8px'}}>
                            {project.ProjectStatus}
                          </span>
                        </td>
                        <td>
                          {variables.PROJECT_IMAGES_URL + project.ProjectImage  ? (
                                 <div style={{width:'50px', height:'50px', margin:'auto'}}> 
                                 <img 
                                   src={variables.PROJECT_IMAGES_URL + project.ProjectImage} 
                                   alt="Project" 
                                     style={{
                                     position:'relative', 
                                     width:'100%', 
                                     height:'100%', 
                                     objectFit:'cover'
                                     }}
                                   />
                                   </div>)
                                   : 
                                 (<div style={{
                                     width:'50px', 
                                     height:'50px', 
                                     margin:'auto', 
                                     background:'#f0f0f0', 
                                     display:'flex', 
                                     alignItems:'center', 
                                     justifyContent:'center',
                                     borderRadius:'4px'
                                   }}>
                                  <span style={{fontSize:'12px', color:'#999'}}>لا توجد صورة</span>
                                   </div>)}
                        </td>
                      </tr>
                    ))
                  )}
                </tbody>
              </table>
            </div>
          </div>

          <div className="footer_s">
            <div style={{ display: 'flex', justifyContent: 'flex-end' }}>
              <button 
                className='btn btn-danger btn_closes'
                onClick={() => dispatch(toggleSearchModal(false))}
              >إغلاق النافذة</button>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}

export default SearchProjectsModal;