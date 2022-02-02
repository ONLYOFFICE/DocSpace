import React from 'react';
import Loaders from '@appserver/common/components/Loaders';
import { inject, observer } from 'mobx-react';

const CatalogHeaderContent = ({ isVisitor, isLoaded, currentModuleName }) => {
  return !isVisitor && (isLoaded ? <>{currentModuleName}</> : <Loaders.ArticleHeader />);
};

export default inject(({ auth }) => {
  return {
    isVisitor: auth.userStore.user.isVisitor,
    isLoaded: auth.isLoaded,
    currentModuleName: auth.product.title,
  };
})(observer(CatalogHeaderContent));
