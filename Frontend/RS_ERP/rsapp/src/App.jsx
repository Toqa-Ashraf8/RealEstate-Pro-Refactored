import './App.css'
import 'animate.css';
import { Routes,Route, Navigate }from 'react-router-dom'
import Header from './assets/nav/Header'
import { ToastContainer,toast } from 'react-toastify';
import Register from './features/user/Register';
import Login from './features/user/Login';
import DashboardPage from './features/dashboard/DashboardPage';
import AddProjects from './features/projects/pages/AddProjects';
import ProjectsReview from './features/projects/pages/ProjectsReview';
import UnitsCard from './features/units/UnitsCard';
import BookingsManager from './features/booking/complete/BookingsManager';
import NegotiationRequests from './features/negotiations/pages/NegotiationRequests'
import AddClients from './features/clients/pages/AddClients';
import RejectedNegotiations from './features/negotiations/pages/RejectedNegotiations';
import AcceptedNegotiations from './features/negotiations/pages/AcceptedNegotiations';
import CompleteBooking from './features/booking/complete/CompleteBooking';
import InstallmentsSchedule from './features/booking/complete/InstallmentsSchedule';
import BookedClients from './features/booking/booked/BookedClients';
import ClientsPage from './features/dashboardClient/ClientsPage';
import BookingsReport from './assets/reports/BookingsReport';
import ProtectedRoute from './components/ProtectedRoute';
import 'react-toastify/dist/ReactToastify.css';
import { useDispatch, useSelector } from 'react-redux';
import { useEffect, useRef } from 'react';
import {clearGlobalError} from './assets/redux/uiSlice'
import ClientDetails from './features/dashboardClient/ClientDetails';


function App() {
const dispatch = useDispatch();
const {token}=useSelector((state)=>state.auth);
const { 
  isLoading, 
  globalError, 
  globalMessage 
} = useSelector((state) => state.ui);
const componentRef = useRef();
const {
  bookingClient,
  installmentDetails
}=useSelector((state)=>state.booking);
useEffect(() => {
    if (globalError) {
      toast.error(globalMessage || "حدث خطأ في السيرفر",);
      dispatch(clearGlobalError());
    }
  }, [globalError, globalMessage, dispatch]);

  return (
    <>
      <ToastContainer 
        position="top-left" 
        autoClose={3000} 
        rtl={true} 
        pauseOnFocusLoss
        draggable
        pauseOnHover
      />

    <Header />
      <Routes>    
             <Route path="/" element={<Navigate to="/login" replace />} />  
              <Route path='/register' element={<Register/>}/>
              <Route path='/login' element={<Login/>}/>
           
            <Route path="/dashboard" 
              element={
              <ProtectedRoute allowedRoles={['Admin']}>  
                <DashboardPage/>
              </ProtectedRoute>
              }
              />
           
             <Route path="/addprojects" 
              element={
              <ProtectedRoute allowedRoles={['Admin']} >  
                <AddProjects/>
                </ProtectedRoute>
              }
              />
              <Route path="/projects" 
              element={
                <ProtectedRoute>
                  <ProjectsReview/>
              </ProtectedRoute>
             }
              />
              <Route path="/units" 
              element={
                <ProtectedRoute>
                  <UnitsCard/>
                </ProtectedRoute>
               }
              />
              <Route path='/addclients' 
              element={
                <ProtectedRoute>
                  <AddClients/>
                 </ProtectedRoute>
              }/> 
              <Route path='/negotiation_requests' 
              element={
                <ProtectedRoute allowedRoles={['Admin']}>
                 <NegotiationRequests/>
                 </ProtectedRoute>
              }/>
              <Route path='/booking' 
              element={
                 <ProtectedRoute>
                   <BookingsManager/>
                 </ProtectedRoute>
              }
              /> 
              <Route path='/rejected_negotiations' 
              element={
                <ProtectedRoute allowedRoles={['Admin']}>
                 <RejectedNegotiations/>
                </ProtectedRoute>
               }
              /> 
              <Route path='/accepted_negotiations' 
              element={
                 <ProtectedRoute allowedRoles={['Admin']}>
                  <AcceptedNegotiations/>
                 </ProtectedRoute>
              }
              />  
              <Route path='/complete_booking' 
              element={
               <ProtectedRoute>
                <CompleteBooking/>
              </ProtectedRoute>
              }
              />
              <Route path='/installments_schedule' 
              element={
               <ProtectedRoute>
                  <InstallmentsSchedule/>
               </ProtectedRoute>
              }
              />
              <Route path='/booked_clients' 
              element={
                 <ProtectedRoute>
                  <BookedClients/>
                  </ProtectedRoute>
               }
              /> 
              <Route path='/clientpage' 
              element={
                 <ProtectedRoute>
                    <ClientsPage/>
                  </ProtectedRoute>
               }
              /> 
              <Route path='/clientdetails' 
              element={
                 <ProtectedRoute>
                    <ClientDetails/>
                  </ProtectedRoute>
               }
              /> 
              <Route path='/bookingreport' 
               element={
                 <ProtectedRoute>
                     <BookingsReport 
                    ref={componentRef} 
                    client={bookingClient} 
                    installments={installmentDetails}
                  /> 
                  </ProtectedRoute>
                }
              />
      </Routes>
    </>
  )
}

export default App
