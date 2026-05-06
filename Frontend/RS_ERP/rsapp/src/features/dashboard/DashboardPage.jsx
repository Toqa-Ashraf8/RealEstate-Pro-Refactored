import React, { useEffect } from 'react';
import './DashboardPage.css'; 
import { useDispatch, useSelector } from 'react-redux';
import ProjectsUnitsBarChart from '../../components/ProjectsUnitsBarChart'; 
import DailyBookingChart from '../../components/DailyBookingChart';
import { 
  availableUnitsCount,
  fetchClientsCount, 
  fetchMonthlyReservations, 
  fetchNegotiationsCount, 
  fetchProjectsCount, 
  fetchProjectsUnitsStats, 
  reservedUnitsCount
} from "../../services/dashboardServices";
const DashboardPage = () => {
  const dispatch=useDispatch();
  const {
    projectsCount,
    clientsCount,
    negotiationsCount,
    reservedUnits,
    availableUnits
  }=useSelector((state)=>state.dashboard);
  const stats = [
    { title: "عدد المشاريع", value: projectsCount },
    { title: "إجمالي العملاء", value: clientsCount },
    { title: "طلبات الشراء", value: negotiationsCount},
    { title: "وحدات محجوزة", value: reservedUnits },
    { title: "وحدات متاحة", value: availableUnits },
  ];


useEffect(()=>{
  const fetchData=async()=>{
   await Promise.all([
      dispatch(fetchProjectsUnitsStats()),
      dispatch(fetchMonthlyReservations()),
      dispatch(fetchProjectsCount()),
      dispatch(fetchClientsCount()),
      dispatch(fetchNegotiationsCount()),
      dispatch(reservedUnitsCount()),
      dispatch(availableUnitsCount())
    ]);
  }
  fetchData();
},[dispatch])

  return (
    <div className="dashboard-main-wrapper" dir="rtl">
      <div className="stats-container">
        {stats.map((item, index) => (
          <div key={index} className="custom-stat-box">
            <span className="box-label">{item.title}</span>
            <h3 className="box-number">{item.value}</h3>
          </div>
        ))}
      </div>

  
      <div className="charts-flex-row">
        <div className="chart-item-box">
          <div className="chart-inner-card">
            <div className="chart-top-bar">
              <h5 className="chart-main-heading">أكثر المشاريع وحدات</h5>
            </div>
            <div className="chart-visual-area">
               <ProjectsUnitsBarChart /> 
            </div>
          </div>
        </div>

        <div className="chart-item-box">
          <div className="chart-inner-card">
            <div className="chart-top-bar">
              <h5 className="chart-main-heading">نشاط الحجوزات اليومي</h5>
            </div>
            <div className="chart-visual-area">
              <DailyBookingChart /> 
            </div>
          </div>
        </div>     
      </div>  
    </div>
  );
};

export default DashboardPage;