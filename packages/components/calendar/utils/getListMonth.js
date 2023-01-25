//return list of months with disabled flag
// ex: [{key: '1', label: 'February', disabled: true}, ...]

export const getListMonth = (minDate, maxDate, openToDate, months) => {
  const minDateYear = minDate.getFullYear();
  const minDateMonth = minDate.getMonth();

  const maxDateYear = maxDate.getFullYear();
  const maxDateMonth = maxDate.getMonth();

  const openToDateYear = openToDate.getFullYear();

  const listMonths = [];

  let i = 0;
  while (i <= 11) {
    listMonths.push({
      key: `${i}`,
      label: `${months[i]}`,
      disabled: false,
    });
    i++;
  }

  if (openToDateYear < minDateYear) {
    i = 0;
    while (i != 12) {
      if (i != minDateMonth) listMonths[i].disabled = true;
      i++;
    }
  }

  if (openToDateYear === minDateYear) {
    i = 0;
    while (i != minDateMonth) {
      listMonths[i].disabled = true;
      i++;
    }
  }

  if (openToDateYear === maxDateYear) {
    i = 11;
    while (i != maxDateMonth) {
      listMonths[i].disabled = true;
      i--;
    }
  }

  return listMonths;
};
