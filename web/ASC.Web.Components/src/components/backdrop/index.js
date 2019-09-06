import React from 'react'
import PropTypes from 'prop-types'
import styled from 'styled-components'

const StyledBackdrop = styled.div`
  background-color: rgba(0, 0, 0, 0.3);
  display: ${props => props.visible ? 'block' : 'none'};
  height: 100vh;
  position: fixed;
  width: 100vw;
  z-index: ${props => props.zIndex};
  left: 0;
  top: 0;
`;

const Backdrop = props => { 
  //console.log("Backdrop render");
  return (
    <StyledBackdrop {...props}/> 
  );
}

Backdrop.propTypes = {
  visible: PropTypes.bool,
  zIndex: PropTypes.number
};

Backdrop.defaultProps = {
  visible: false,
  zIndex: 500
};

export default Backdrop;