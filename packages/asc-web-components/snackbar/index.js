import React, { useCallback } from "react";
import PropType from "prop-types";
import StyledSnackBar from "./styled-snackbar";
import StyledCrossIcon from "./styled-snackbar-action";
import StyledLogoIcon from "./styled-snackbar-logo";
import Box from "../box";
import Heading from "../heading";
import Text from "../text";

const SnackBar = ({ text, headerText, onAction, ...rest }) => {
  const onActionClick = useCallback(
    (e) => {
      onAction && onAction(e);
    },
    [onAction]
  );

  const headerStyles = headerText ? {} : { display: "none" };

  return (
    <StyledSnackBar {...rest}>
      <Box className="logo">
        <StyledLogoIcon size="medium" />
      </Box>
      <Box className="text-container">
        <Heading
          size="xsmall"
          isInline={true}
          className="text-header"
          style={headerStyles}
        >
          {headerText}
        </Heading>
        <Text>{text}</Text>
      </Box>
      <button className="action" onClick={onActionClick}>
        <StyledCrossIcon size="medium" />
      </button>
    </StyledSnackBar>
  );
};

SnackBar.propTypes = {
  text: PropType.string,
  headerText: PropType.string,
  onAction: PropType.func,
};

export default SnackBar;
