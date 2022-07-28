import equal from "fast-deep-equal/es6/react";
import isEmpty from "lodash/isEmpty";

export const isArrayEqual = (arr1, arr2) => {
  return isEmpty(equal(arr1, arr2));
};
