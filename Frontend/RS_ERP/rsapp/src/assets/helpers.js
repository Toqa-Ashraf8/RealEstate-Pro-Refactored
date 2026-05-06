export const formatCurrency = (value) => {
  if (!value) return "0";
  return Number(value).toLocaleString('en-US', {
    maximumFractionDigits: 0 
  });
};
/* Number(value).toLocaleString('',{maximumFractionDigits:0}) */