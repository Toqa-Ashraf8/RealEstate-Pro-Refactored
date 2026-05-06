import React from 'react';
import { Bar } from 'react-chartjs-2';
import { Chart as ChartJS, CategoryScale, LinearScale, BarElement, Tooltip, Legend } from 'chart.js';
import { useSelector } from 'react-redux';

ChartJS.register(CategoryScale, LinearScale, BarElement, Tooltip, Legend);

const ProjectsUnitsBarChart = ({ projectsData }) => {
const {projectsUnitsCounts}=useSelector((state)=>state.dashboard);
  const data = { 
      labels: projectsUnitsCounts.map((item) => item.ProjectName),
     datasets: [
      {
        data: projectsUnitsCounts.map((item)=>item.TotalUnitsCount), 
        backgroundColor: '#0d6efd', 
        borderRadius: 8, 
        borderSkipped: false,
      },
    ],
  };

  const options = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { display: false }, 
    },
    scales: {
      y: {
        beginAtZero: true,
        position: 'right', 
      },
      x: {
        reverse: true, 
      }
    }
  };

  return <Bar data={data} options={options} />;
};

export default ProjectsUnitsBarChart;