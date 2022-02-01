import React from 'react';
import PropTypes from 'prop-types';
import IconButton from '@appserver/components/icon-button';

const ArrowButton = ({ isRootFolder, onBackToParentFolder }) => {
  return (
    !isRootFolder && (
      <IconButton
        iconName="/static/images/arrow.path.react.svg"
        size="17"
        color="#A3A9AE"
        hoverColor="#657077"
        isFill={true}
        onClick={onBackToParentFolder}
        className="arrow-button"
      />
    )
  );
};

export default React.memo(ArrowButton);
