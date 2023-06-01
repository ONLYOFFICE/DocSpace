const getCorrectBorderRadius = (borderRadiusStr, interfaceDirection) => {
  if (interfaceDirection === "ltr") return borderRadiusStr;

  const borderRadiusArr = borderRadiusStr.split(" ");

  switch (borderRadiusArr.length) {
    // [10px] => "10px"
    case 1: {
      return borderRadiusStr;
    }
    // [10px 20px] => [20px 10px]
    case 2: {
      borderRadiusArr.splice(0, 0, borderRadiusArr.splice(1, 1)[0]);
      break;
    }
    // [10px 20px 30px] => [20px 10px 20px 30px]
    case 3: {
      borderRadiusArr.splice(0, 0, borderRadiusArr[1]);
      break;
    }
    // [10px 20px 30px 40px] => [20px 10px 40px 30px]
    case 4: {
      borderRadiusArr.splice(0, 0, borderRadiusArr.splice(1, 1)[0]);
      borderRadiusArr.splice(2, 0, borderRadiusArr.splice(3, 1)[0]);
      break;
    }
  }

  return borderRadiusArr.join(" ");
};

export default getCorrectBorderRadius;
