import React from "react";
import Loaders from "@docspace/common/components/Loaders";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import withLoader from "../../../HOCs/withLoader";

const ArticleHeaderContent = ({ currentModuleName }) => {
  return <>{currentModuleName}</>;
};

export default inject(({ auth }) => {
  return {
    currentModuleName: (auth.product && auth.product.title) || "",
  };
})(
  withTranslation([])(
    withLoader(observer(ArticleHeaderContent))(<Loaders.ArticleHeader />)
  )
);
