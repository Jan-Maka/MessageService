export const formatDate = (date) => {
    const newDate = new Date(date);
    return `${newDate.toLocaleString().slice(0,-3)}`;
};