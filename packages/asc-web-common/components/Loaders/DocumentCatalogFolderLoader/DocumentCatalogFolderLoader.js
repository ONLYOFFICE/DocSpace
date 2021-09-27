import React from 'react';
import PropTypes from 'prop-types';
import RectangleLoader from '../RectangleLoader';
import { StyledContainer, StyledBlock } from './StyledDocumentCatalogFolderLoader';

const DocumentCatalogFolderLoader = ({ id, className, style, ...rest }) => {
  return (
    <StyledContainer>
      <StyledBlock id={id} className={className} style={style}>
        <RectangleLoader width="100%" height="36px" {...rest} />
        <RectangleLoader width="100%" height="36px" {...rest} />
        <RectangleLoader width="100%" height="36px" {...rest} />
        <RectangleLoader width="100%" height="36px" {...rest} />
      </StyledBlock>
      <StyledBlock>
        <RectangleLoader width="100%" height="36px" {...rest} />
        <RectangleLoader width="100%" height="36px" {...rest} />
      </StyledBlock>
      <StyledBlock>
        <RectangleLoader width="100%" height="36px" {...rest} />
      </StyledBlock>
      <StyledBlock>
        <RectangleLoader width="100%" height="36px" {...rest} />
      </StyledBlock>
    </StyledContainer>
  );
};

DocumentCatalogFolderLoader.propTypes = {
  id: PropTypes.string,
  className: PropTypes.string,
  style: PropTypes.object,
};

DocumentCatalogFolderLoader.defaultProps = {
  id: undefined,
  className: undefined,
  style: undefined,
};

export default DocumentCatalogFolderLoader;
