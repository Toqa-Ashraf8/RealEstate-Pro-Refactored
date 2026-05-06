import React, { useRef } from "react";
import { motion } from "framer-motion";
import {
  Building2,
  MapPin,
  Hash,
  Layers,
  Image as ImageIcon,
  CheckCircle2,
} from "lucide-react";
import "./AddProjects.css";
import { AiOutlineClear } from "react-icons/ai";
import { MdAdd, MdDeleteOutline, MdOutlineDomainAdd } from "react-icons/md";
import { RiSave3Fill, RiDeleteBinLine } from "react-icons/ri";
import { FaSearch } from "react-icons/fa";
import { useDispatch, useSelector } from "react-redux";
import {
  prepareUnitModal,
  resetProjectForm,
  setProjectData,
  SetRowIndexvalue,
  showDeleteUnitModal,
  toggleDeleteProjectModal,
  toggleSearchModal,
  toggleUnitModal,
} from "../../../assets/redux/projectSlice.js"; 
import { CiEdit } from "react-icons/ci";
import { variables } from "../../../assets/variables.js"; 
import { toast } from "react-toastify";
import { formatCurrency } from "../../../assets/helpers.js";
import UnitDeleteModal from "../../units/UnitDeleteModal.jsx";
import UnitModal from "../../units/UnitModal.jsx";
import SearchProjectsModal from "../modals/SearchProjectsModal.jsx";
import ProjectDeleteModal from "../modals/ProjectDeleteModal.jsx";
import { saveCompleteProject, uploadProjectImage } from "../../../services/projectService.js";

const AddProjects = () => {
  const {
    project,
    isUnitModalOpen,
    unitsList,
    isDeleteUnitModalOpen,
    isDeleteProjectModalOpen,
    isSearchModalOpen,
    projectImageName,
    unitFormMode
  } = useSelector((state) => state.projects);
  const dispatch = useDispatch();
  const NameRef = useRef();

 const handleImageUpload = async (e) => {
    const { name } = e.target;
    if (!e.target.files || e.target.files.length === 0) return;
    const file = e.target.files[0];
    const formData = new FormData();
    const fileName = file.name;
    formData.append("file", file, fileName);
    await dispatch(uploadProjectImage(formData));
    await dispatch(setProjectData({ [name]: fileName }));
  };

    const handleInputChange = (e) => {
    const { name, value } = e.target;
    dispatch(setProjectData({ [name]: value }));
  };
  
  const handleResetForm = () => {
    dispatch(resetProjectForm());
    NameRef.current.focus();
  };

 const handleSaveProject = async () => {
     const parms = { ...project, units:unitsList.map(item=>({...item,ReservedStatus:0}))};
     try {
      const result = await dispatch(saveCompleteProject(parms)).unwrap();
      if (result.errorOccured) {
        toast.error("أدخل بيانات لإتمام عملية الحفظ!", {
          theme: "colored",
          position: "top-left",
        });
      } else {
        toast.success("تم الحفظ بنجاح!", {
          theme: "colored",
          position: "top-left",
        });
      }
    } catch (error) {
      toast.error("حدث خطأ في الاتصال بالخادم", {
        theme: "colored",
        position: "top-left",
      });
    } 
  };
  const handleOpenUnitModal = () => {
    dispatch(prepareUnitModal(unitsList.length + 1));
    dispatch(toggleUnitModal(true));
  };
  
  const handleEditUnit = (index) => {
    dispatch(toggleUnitModal(true));
    dispatch(SetRowIndexvalue(index));
  };
  const handleDeleteProject=()=>{
    if(project.ProjectCode===0){
       toast.error("اختر مشروع لحذفه أولا !", {
              theme: "colored",
              position: "top-right"
        });
        return;
    }
    else{
      dispatch(toggleDeleteProjectModal(true));
    }
  }
  return (
    <div className="page-container">
      <div className="add-project-wrapper" dir="rtl">
        {isUnitModalOpen && <UnitModal />}
        {isDeleteUnitModalOpen && <UnitDeleteModal />}
        {isDeleteProjectModalOpen && <ProjectDeleteModal />}
        {isSearchModalOpen && <SearchProjectsModal />}

        <div className="btns_toc_p">
          <button className="icon-btn" onClick={()=>handleResetForm()} title="تنظيف">
            <span className="btn_c"><AiOutlineClear size={22} color="#64748b" /></span>
          </button>
          <button className="icon-btn" onClick={()=>handleResetForm()} title="جديد">
            <span className="btn_c"><MdOutlineDomainAdd size={22} color="#1e40af" /></span>
          </button>
          <button className="icon-btn" onClick={()=>handleSaveProject()} title="حفظ">
            <span className="btn_c"><RiSave3Fill size={22} color="#10b981" /></span>
          </button>
          <button className="icon-btn" onClick={() =>handleDeleteProject()} title="حذف">
            <span className="btn_c"><RiDeleteBinLine size={22} color="#ef4444" /></span>
          </button>
          <button className="icon-btn" onClick={() => dispatch(toggleSearchModal(true))} title="بحث">
            <span className="btn_c"><FaSearch size={20} color="#3b82f6" /></span>
          </button>
        </div>

        <div className="form-header">
          <h2 className="p_title">إضافة مشروع عقاري جديد</h2>
        </div>

        <motion.div initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }} className="main_cnt">
          <div className="row g-5">
            <div className="col-lg-8">
              <div className="row">
  
                <div className="col-12 data_cnt">
                  <label className="data_lbl"><Hash size={18} /> كود المشروع</label>
                  <input 
                  type="text" 
                  className="form-control-modern inp_code" 
                  value={project.ProjectCode || 0} 
                  disabled />
                </div>

                <div className="col-12 data_cnt">
                  <label className="data_lbl"><Building2 size={18} /> اسم المشروع</label>
                  <input 
                  type="text" 
                  className="form-control-modern" 
                  ref={NameRef} 
                  name="ProjectName" 
                  value={project.ProjectName || ""} 
                  onChange={handleInputChange} />
                </div>

                <div className="col-12 data_cnt">
                  <label className="data_lbl"><Building2 size={18} /> نوع المشروع</label>
                  <select 
                  className="form-select-modern" 
                  name="ProjectType" 
                  value={project.ProjectType || ""} 
                  onChange={handleInputChange}>
                    <option value="-1">-- إختر النوع --</option>
                    <option value="تجاري">تجاري</option>
                    <option value="إداري">إداري</option>
                    <option value="طبي">طبي</option>
                    <option value="سكني">سكني</option>
                  </select>
                </div>

                <div className="col-12 data_cnt">
                  <label className="data_lbl"><MapPin size={18} /> الموقع الجغرافي</label>
                  <input 
                  type="text" 
                  className="form-control-modern" 
                  name="Location" 
                  value={project.Location || ""} 
                  onChange={handleInputChange}
                   />
                </div>

                <div className="col-12 data_cnt">
                  <label className="data_lbl"><Layers size={18} /> إجمالي الوحدات</label>
                  <input 
                  type="number" 
                  className="form-control-modern" 
                  name="TotalUnits" 
                  value={project.TotalUnits || 0} 
                  onChange={handleInputChange} 
                  />
                </div>

                <div className="col-12 data_cnt">
                  <label className="data_lbl"><CheckCircle2 size={18} /> حالة المشروع</label>
                  <select 
                  className="form-select-modern" 
                  name="ProjectStatus" 
                  value={project.ProjectStatus || ""} 
                  onChange={handleInputChange}>
                    <option value="-1">-- اختر الحالة --</option>
                    <option value="قيد الإنشاء">قيد الإنشاء</option>
                    <option value="في مرحلة التشطيبات">في مرحلة التشطيبات</option>
                    <option value="قريبا">قريبا</option>
                    <option value="جاهز للاستلام">جاهز للاستلام</option>
                  </select>
                </div>

                <div className="col-12 data_cnt">
                  <label className="data_lbl"><ImageIcon size={18} /> صورة المشروع</label>
                  <div className="file-input-wrapper">
                    <input type="file" name="ProjectImage" onChange={handleImageUpload} />
                    <div className="custom-file-label">
                      <span>اضغط لرفع صورة المشروع</span>
                      <ImageIcon size={20} />
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <div className="col-lg-4">
              <div className="img_cnt">
                {(() => {
                const imgName = projectImageName || project?.ProjectImage;
                  if (imgName && imgName !== "null") {
                   return (
                    <img 
                     src={`${variables.PROJECT_IMAGES_URL}/${imgName}`} 
                     className="preview-img"
                     alt="" 
                    />
                    );
                   } else {
                     return (
                        <div className="final_empty_msg_p">
                        <ImageIcon size={40} className="final_icon_fade" />
                         <p style={{color:'#d0d0d0'}}>معاينة المشروع</p>
                        </div>
                      );
                    }
                  })()}
              </div>
            </div>
          </div>
        </motion.div>

        <div className="btn_addunits">
          <button className="btn_add_units" onClick={() => handleOpenUnitModal()}>
            <MdAdd size={24} /> إضافة وحدات للمشروع
          </button>
        </div>

        <div className="tbl_units">
          <table className="table tbl_u">
            <thead>
              <tr>
                <th>كود</th>
                <th>الوحدة</th>
                <th>الدور</th>
                <th>المساحة</th>
                <th>سعر المتر</th>
                <th>الإجمالي</th>
                <th>الصورة</th>
                <th>الأحداث</th>
              </tr>
            </thead>
            <tbody>
              {unitsList.length === 0 ? (
                <tr><td colSpan={8} style={{color:'#d0d0d0'}}>لا توجد وحدات مضافة حالياً</td></tr>
              ) : (
                unitsList.map((unit, index) => (
                  <tr key={index}>
                    <td>{unit.serial}</td>
                    <td>{unit.unitName}</td>
                    <td>{unit.Floor}</td>
                    <td>{unit.TotalArea} م²</td>
                    <td>{unit.MeterPrice}</td>
                    <td>{formatCurrency(unit.TotalPrice)}</td>
                    <td> {variables.UNIT_IMAGES_URL && unit.unitImage ? (
                        <div
                          style={{
                            width: "40px",
                            height: "40px",
                            margin: "auto",
                          }}
                        >
                          <img
                            src={variables.UNIT_IMAGES_URL + (unit.unitImage || unitImageName)}
                            alt="Unit"
                            style={{
                              position: "relative",
                              width: "100%",
                              height: "100%",
                              objectFit: "cover",
                            }}
                          />
                        </div>
                      ) : (
                      
                        <div
                          style={{
                            width: "40px",
                            height: "40px",
                            margin: "auto",
                            background: "#f0f0f0",
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "center",
                            borderRadius: "4px",
                          }}
                        >
                          <span style={{ fontSize: "12px", color: "#999" }}>
                            لا توجد صورة
                          </span>
                        </div> 
                    )}</td>
                    <td>
                      <div className="btns_dtls" style={{ display: 'flex', gap: '20px', justifyContent: 'center' }}>
                        <CiEdit size={24} color="#1e40af" style={{ cursor: 'pointer' }} onClick={() => { dispatch(toggleUnitModal(true)); dispatch(SetRowIndexvalue(index)); }} />
                        <MdDeleteOutline size={24} color="#ef4444" style={{ cursor: 'pointer' }} onClick={() => dispatch(showDeleteUnitModal(index))} />
                      </div>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
};

export default AddProjects;