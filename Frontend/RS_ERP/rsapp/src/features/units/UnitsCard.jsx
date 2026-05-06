import React, { useEffect } from 'react'
import './UnitsCard.css'
import { useDispatch, useSelector } from 'react-redux'
import { FaRulerCombined, FaTag, FaImage, FaInfoCircle } from 'react-icons/fa';
import { variables } from '../../assets/variables';
import { formatCurrency } from '../../assets/helpers.js'
import { HandCoins } from 'lucide-react';
import { fetchProjectUnits } from '../../services/projectService.js';

const UnitsCard = () => {
  const projectState = useSelector((state) => state.projects);
  const dispatch = useDispatch();

  useEffect(() => {
    const Getunits = async () => {
      if (projectState.selectedProjectCode) {
        await dispatch(fetchProjectUnits(projectState.selectedProjectCode));
      }
    }
    Getunits();
  }, [projectState.selectedProjectCode]);

  return (
    <div className="u-card-wrapper" dir="rtl">
      <div className="u-card-container">
        {projectState.displayUnitsList.length === 0 ? (
          <div className="u-card-empty">
            <FaImage size={40} color="#ccc" />
            <p>لا توجد وحدات متاحة لهذا المشروع حالياً</p>
          </div>
        ) : (
          <div className="u-card-grid">
            {projectState.displayUnitsList.map((unit, index) => (
              <div key={index} className="u-card-item">
                <div className="u-card-header">
                  {unit.unitImage ? (
                    <img src={variables.UNIT_IMAGES_URL + unit.unitImage} 
                    alt={unit.unitName} className="u-card-img" 
                    />
                  ) : (
                    <div className="u-card-no-img"><FaImage size={24} /></div>
                  )}
                  <div className="u-card-badge">{unit.Floor}</div>
                </div>

                <div className="u-card-info">
                  <div style={{display:'flex',justifyContent:'space-between'}}>
                 <span><h4 className="u-card-title">{unit.unitName}</h4></span> 
                  <div className='u-card-status'>
                    {unit.ReservedStatus ? 
                    (<span style={{fontWeight:'bold',color:'red'}}>محجوزة</span>)
                     :
                     (<span style={{fontWeight:'bold',color:'green'}}>متاحة</span>)
                    }
                  
                  </div>
                  </div>
                  <div className="u-card-details">
                    <div className="u-card-detail">
                      <FaRulerCombined size={12} />
                      <span>{unit.TotalArea} م²</span>
                    </div>
                    <div className="u-card-detail">
                      <HandCoins size={12} />
                      <span>{unit.MeterPrice} ج/م</span>
                    </div>
                  </div>

                  <div className="u-card-price-row">
                    <div className="u-card-price">
                      <span className="price-label">السعر الإجمالي</span>
                      <span className="price-value">
                        {formatCurrency(unit.TotalPrice)} ج
                        </span>
                    </div>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  )
}

export default UnitsCard;