import React, { useCallback } from "react";
import PropType from "prop-types";
import StyledSnackBar from "./styled-snackbar";
import StyledCrossIcon from "./styled-snackbar-action";
import StyledLogoIcon from "./styled-snackbar-logo";
import Box from "../box";
import Heading from "../heading";
import Text from "../text";

const SnackBar = ({
  text,
  headerText,
  btnText,
  onAction,
  textColor,
  ...rest
}) => {
  const onActionClick = useCallback(
    (e) => {
      onAction && onAction(e);
    },
    [onAction]
  );

  const headerStyles = headerText ? {} : { display: "none" };

  console.log("Snackbar render");
  return (
    <StyledSnackBar {...rest}>
      <Box className="logo">
        <StyledLogoIcon size="medium" textColor={textColor} />
      </Box>
      <Box className="text-container">
        <Heading
          size="xsmall"
          isInline={true}
          className="text-header"
          style={headerStyles}
          color={textColor}
        >
          {headerText}
        </Heading>
        <Text color={textColor}>{text}</Text>
      </Box>
      <button className="action" onClick={onActionClick}>
        {btnText ? btnText : <StyledCrossIcon size="medium" />}
      </button>
    </StyledSnackBar>
  );
};

SnackBar.propTypes = {
  text: PropType.string,
  headerText: PropType.string,
  btnText: PropType.string,
  backgroundImg: PropType.string,
  backgroundColor: PropType.string,
  textColor: PropType.string,
  onAction: PropType.func,
};

SnackBar.defaultProps = {
  backgroundColor: "#f8f7bf",
  textColor: "#000",
};

export default SnackBar;
