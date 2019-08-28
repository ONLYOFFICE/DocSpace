import React from "react";
import IconButton from '../icon-button';
import styled from 'styled-components';

const StyledCloseButton = styled.div`
    margin-left: 7px;
    margin-top: -1px;
`;
const CloseButton = props => {
  //console.log("CloseButton render");
  return (
      <StyledCloseButton className={props.className}>
          <IconButton
            color={"#D8D8D8"}
            hoverColor={"#333"}
            clickColor={"#333"}
            size={10}
            iconName={'CrossIcon'}
            isFill={true}
            isDisabled={props.isDisabled}
            onClick={!props.isDisabled ? ((e) => props.onClick()) : undefined}
        />
      </StyledCloseButton>
  );
};

export default CloseButton