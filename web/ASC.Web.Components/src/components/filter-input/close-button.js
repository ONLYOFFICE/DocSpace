import React from "react";
import IconButton from '../icon-button';
import styled from 'styled-components';
import PropTypes from 'prop-types';

const StyledCloseButton = styled.div`
    margin-left: 7px;
    margin-top: -1px;
`;
const CloseButton = props => {
  //console.log("CloseButton render");
  return (
    <StyledCloseButton className={props.className}>
      <IconButton
        color={"#A3A9AE"}
        hoverColor={"#A3A9AE"}
        clickColor={"#A3A9AE"}
        size={10}
        iconName={'CrossIcon'}
        isFill={true}
        isDisabled={props.isDisabled}
        onClick={!props.isDisabled ? props.onClick : undefined}
      />
    </StyledCloseButton>
  );
};
CloseButton.propTypes = {
  isDisabled: PropTypes.bool,
  onClick: PropTypes.func,
  className: PropTypes.string
}
export default CloseButton