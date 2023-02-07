import differenceWith from "lodash/differenceWith";
import isEqual from "lodash/isEqual";
import isEmpty from "lodash/isEmpty";

export const isArrayEqual = (arr1, arr2) => {
  return isEmpty(differenceWith(arr1, arr2, isEqual));
};
