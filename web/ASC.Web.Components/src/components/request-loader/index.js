import React from "react";
import PropTypes from 'prop-types';
import styled from 'styled-components';
import Loader from '../loader';

const StyledOuter = styled.div`
  position: fixed;
  text-align: center;
  top: 10px;
  width: 100%;
  z-index: ${props => props.zIndex};
  display: ${props => props.visible ? 'block' : 'none'};
`;

const StyledInner = styled.div`
  background-color: #fff;
  border: 1px solid #cacaca;
  color: ${props => props.fontColor};
  display: inline-block;
  font-size: ${props => props.fontSize}px;
  white-space: nowrap;
  overflow: hidden;
  padding: 5px 10px;
  line-height: 16px;
  z-index: ${props => props.zIndex};
  border-radius: 5px;
  -moz-border-radius: 5px;
  -webkit-border-radius: 5px;
  box-shadow: 0 2px 8px rgba(0,0,0,.3);
  -moz-box-shadow: 0 2px 8px rgba(0,0,0,.3);
  -webkit-box-shadow: 0 2px 8px rgba(0,0,0,.3);
`;

const OvalLoader = styled(Loader)`
  display: inline;
  margin-right: 10px;
`;

const RequestLoader = props => {
  //console.log("RequestLoader render");
  const { loaderColor, loaderSize, label } = props;
  return (
    <StyledOuter {...props}>
      <StyledInner {...props}>
        <OvalLoader type="oval" color={loaderColor} size={loaderSize} label={label} />
        {label}
      </StyledInner>
    </StyledOuter>
  );
};

RequestLoader.propTypes = {
  visible: PropTypes.bool,
  zIndex: PropTypes.number,  
  loaderSize: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  loaderColor: PropTypes.string,
  label: PropTypes.string,
  fontSize: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  fontColor: PropTypes.string,
};

RequestLoader.defaultProps = {
  visible: false,
  zIndex: 256,
  loaderSize: 16,
  loaderColor: '#999',
  label: 'Loading... Please wait...',
  fontSize: 12,
  fontColor: '#999'
};

export default RequestLoader;