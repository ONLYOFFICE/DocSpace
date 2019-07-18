import React from "react";
import IconButton from '../icon-button';

const CloseButton = props => {
  return (
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
  );
};

export default CloseButton