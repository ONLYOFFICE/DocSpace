export const getKeyByLink = (data, linkArr) => {
 const length = linkArr.length;
 if (length === 1 || !linkArr[1].length) {
   const arrLength = data.length;
   for (let i = 0; i < arrLength; i++) {
     if (data[i].link === linkArr[0]) {
       return data[i].children ? data[i].children[0].key : data[i].key;
     }
   }
 } else if (length === 2) {
   const arrLength = data.length;
   let key;

   for (let i = 0; i < arrLength; i++) {
     if (data[i].link === linkArr[0]) {
       key = i;
       break;
     }
   }

   const selectedArr = data[key].children;
   const childrenLength = selectedArr.length;
   for (let i = 0; i < childrenLength; i++) {
     if (selectedArr[i].link === linkArr[1]) {
       return selectedArr[i].key;
     }

   }
 }
 return '0-0';
}
