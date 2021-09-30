import React from 'react';

const CatalogBody = (props) => {
  return <> {props.children}</>;
};

CatalogBody.displayName = 'CatalogBody';

export default React.memo(CatalogBody);
