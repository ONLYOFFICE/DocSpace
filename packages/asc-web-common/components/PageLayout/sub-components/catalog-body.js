import React from 'react';
import styled from 'styled-components';
import PropTypes from 'prop-types';

const CatalogBody = (props) => {
  return <> {props.children}</>;
};

CatalogBody.displayName = 'CatalogBody';

export default React.memo(CatalogBody);
