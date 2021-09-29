import React from 'react';
import styled from 'styled-components';
import PropTypes from 'prop-types';
import { Resizable } from 're-resizable';
import { isMobile } from 'react-device-detect';
import { mobile, tablet, isTablet } from '@appserver/components/utils/device';

const StyledCatalog = styled.div`
  @media (hover: none) {
    position: relative;
    top: 48px;
  }
  .resizable-block {
    display: flex;
    flex-direction: column;
    min-width: 256px;
    width: 256px;
    height: 100% !important;
    background: #f8f9f9;
    overflow: hidden;
    .resizable-border {
      div {
        cursor: ew-resize !important;
      }
    }

    @media ${tablet} {
      min-width: ${(props) => (props.showText ? '240px' : '52px')};
      max-width: ${(props) => (props.showText ? '240px' : '52px')};
      .resizable-border {
        display: none;
      }
    }

    @media ${mobile} {
      display: ${(props) => (props.showText ? 'flex' : 'none')};
      min-width: 100vw;
      width: 100vw;
      margin: 0;
      padding: 0;
    }
  }
`;

const Catalog = (props) => {
  const { showText, setShowText, children, ...rest } = props;

  const enable = {
    top: false,
    right: !isMobile,
    bottom: false,
    left: false,
  };

  const hideText = React.useCallback((event) => {
    if (isMobile) {
      event.preventDefault;
      setShowText(false);
    }
  }, []);

  React.useEffect(() => {
    window.addEventListener('popstate', hideText);
    return () => window.removeEventListener('popstate', hideText);
  }, [hideText]);

  return (
    <StyledCatalog showText={showText} {...rest}>
      <Resizable
        enable={enable}
        className="resizable-block"
        handleWrapperClass="resizable-border not-selectable">
        {children}
      </Resizable>
    </StyledCatalog>
  );
};

Catalog.propTypes = {
  showText: PropTypes.bool,
  setShowText: PropTypes.func,
  children: PropTypes.any,
};

export default Catalog;
