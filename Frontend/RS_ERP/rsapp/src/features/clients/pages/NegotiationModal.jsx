import React, { useEffect, useRef } from "react";
import "./NegotiationModal.css";
import { Building2, Ungroup, Hash, DollarSign, Percent, X } from 'lucide-react';
import { useDispatch, useSelector } from "react-redux";
import { 
  calculateDiscount,  
  saveNegotiationToTable, 
  setNegotiationData, 
  toggleNegotiationModal 
} from "../../../assets/redux/clientSlice"; 
import { fetchPriceByUnit, fetchProjects, fetchUnitsByProject } from "../../../services/clientService"; 

const NegotiationModal = () => {
  const { negotiation, projects, units } = useSelector((state) => state.clients);
  const dispatch = useDispatch();
  const negotiationPriceRef = useRef();

  const handleInputsChange = (e) => {
    const { name, value } = e.target;
    if (name === "ProjectCode") {
      const selectedProject = projects.find(u => u.ProjectCode === parseInt(value));
      if (value !== "-1") dispatch(fetchUnitsByProject(value));
      if (selectedProject) dispatch(setNegotiationData({ ProjectName: selectedProject.ProjectName }));
    }
    if (name === "UnitID") {
      const selectedUnit = units.find(u => u.UnitID === parseInt(value));
      if (value !== "-1") dispatch(fetchPriceByUnit(value));
      if (selectedUnit) dispatch(setNegotiationData({ unitName: selectedUnit.unitName }));
      setTimeout(() => negotiationPriceRef.current?.focus(), 100);
    }
    dispatch(setNegotiationData({ [name]: value }));
  }

  const addNewNegotiation = () => {
    dispatch(saveNegotiationToTable());
  }

  useEffect(() => {
    dispatch(fetchProjects());
    if (negotiation.ProjectCode !== -1 && negotiation.ProjectCode) {
      dispatch(fetchUnitsByProject(negotiation.ProjectCode));
    }
  }, [dispatch]);

  return (
  <div dir="rtl">
      <div className="modaln">
        <div className="modalcnt_n">
          <div className="headern">
            <div className="mdl_titles">
              <h4 className="units_title">إرسال طلب تفاوض</h4>
              <span className="close_b" onClick={()=>dispatch(toggleNegotiationModal(false))}>
                &times;
              </span>
            </div>
          </div>
          
          <div className="bodyn">
            <div className="row">
                <div className="project-unit-section col-lg-5">
                  <div className="data_field">
                      <label className="data_lbl">إسم المشروع</label>
                      <select 
                      className="crm_select select-project" 
                      name="ProjectCode" 
                      value={negotiation.ProjectCode || ""} 
                      onChange={handleInputsChange} 
                      >
                        <option value="-1">-إختر-</option>
                        {projects.map((project) => 
                        <option key={project.ProjectCode} value={project.ProjectCode}> 
                            {project.ProjectName}
                        </option>
                        )}
                      </select>
                  </div> 

                   {negotiation.ProjectCode && negotiation.ProjectCode !== "-1" && (
                    <div className="data_field mt-3">
                      <label className="data_lbl">الوحدة</label>
                      <select 
                      className="crm_select select-unit" 
                      name="UnitID" 
                      value={negotiation.UnitID || ""} 
                      onChange={handleInputsChange}>
                        <option value="-1">-إختر-</option>
                        {units?.map((unit, index) => 
                        <option key={index} value={unit.UnitID}>
                           {unit.unitName} 
                        </option>
                      )}
                      </select>
                    </div>
                  )}
              </div>

              <div className="col-lg-7 d-flex flex-column gap-3 mt-lg-0 mt-4">
                <div className="input-group-modern data_cntu">
                  <label className="data_lbl">كود الطلب</label>
                  <input
                    type="text"
                    className="form-control-modern"
                    disabled
                    name="serialCode"
                    value={negotiation.serialCode || ""}
                  />
                </div>

                <div className="input-group-modern data_cntu">
                  <label className="data_lbl">السعر الأصلي</label>
                  <input
                    type="text"
                    className="form-control-modern"
                    disabled
                    name="OriginalPrice"
                    value={negotiation.OriginalPrice || ""}
                  />
                </div>

                 <div className="input-group-modern data_cntu">
                  <label className="data_lbl">السعر المقترح</label>
                  <input
                    type="text"
                    className="form-control-modern"
                    ref={negotiationPriceRef}
                    name="NegotiationPrice"
                    value={negotiation.NegotiationPrice || ""}
                    onChange={handleInputsChange}
                    onBlur={() => dispatch(calculateDiscount())}
                  />
                </div>

                 <div className="input-group-modern data_cntu">
                  <label className="data_lbl">قيمة الخصم %</label>
                  <input
                    type="text"
                    className="form-control-modern"
                    name="DiscountAmount"
                    value={negotiation.DiscountAmount || ""}
                    onChange={handleInputsChange}
                  />
                </div>       
              </div>      
            </div>
          </div>

          <div className="footern">
            <div className="d-flex w-100 justify-content-between">
              <button className="btn btn-primary px-5" onClick={()=>addNewNegotiation()}>إضافة</button>
              <button className="btn btn-danger px-5" onClick={()=>dispatch(toggleNegotiationModal(false))}>إلغاء</button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default NegotiationModal;