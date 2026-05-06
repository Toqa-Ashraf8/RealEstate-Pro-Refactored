import React from 'react';
import { Line } from 'react-chartjs-2';
import { Chart as ChartJS, CategoryScale, LinearScale, PointElement, LineElement, Filler } from 'chart.js';
import { useSelector } from 'react-redux';

ChartJS.register(CategoryScale, LinearScale, PointElement, LineElement, Filler);

const DailyBookingChart = ({ bookingData }) => {
  const {reservationstatistics}=useSelector((state)=>state.dashboard);
  const data = {
    labels:reservationstatistics.map(item=>item.MonthName), 
    datasets: [
      {
        fill: true,
        label: 'حجوزات جديدة',
        data: reservationstatistics.map(item=>item.BookingCount), 
        borderColor: '#198754', 
        backgroundColor: 'rgba(25, 135, 84, 0.1)',
        tension: 0.3,
        pointRadius: 6,
        pointHoverRadius: 8,
      },
    ],
  };

  const options = {
    responsive: true,
    maintainAspectRatio: false,
    scales: {
      y: 
      { 
        position: 'right', 
        beginAtZero: true ,
        ticks: {
        stepSize: 1, 
        precision: 0 
      },
      },
      x: { reverse: true }
    }
  };

  return <Line data={data} options={options} />;
};

export default DailyBookingChart;