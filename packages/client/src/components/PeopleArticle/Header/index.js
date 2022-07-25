import React from "react";
import Loaders from "@docspace/common/components/Loaders";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import withLoader from "../../../HOCs/withLoader";

const ArticleHeaderContent = ({ isVisitor, currentModuleName }) => {
  return !isVisitor && <>{currentModuleName}</>;
};

export default inject(({ auth }) => {
  return {
    isVisitor: auth.userStore.user.isVisitor,

    currentModuleName: auth.product.title,
  };
})(
  withTranslation()(
    withLoader(observer(ArticleHeaderContent))(<Loaders.ArticleHeader />)
  )
);
