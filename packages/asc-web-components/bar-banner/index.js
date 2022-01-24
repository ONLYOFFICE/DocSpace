import React, { useState } from "react";
import PropTypes from "prop-types";

import { StyledBarBanner, StyledCrossIcon } from "./styled-bar-banner";

const BarBanner = (props) => {
  const { htmlLink } = props;

  const [open, setOpen] = useState(true);

  const onClose = () => setOpen(false);

  return (
    <StyledBarBanner {...props} open={open}>
      {htmlLink ? (
        <iframe
          height="60px"
          width="100%"
          src={htmlLink}
          frameBorder="0"
          align="left"
          scrolling="no"
        ></iframe>
      ) : (
        ""
      )}
      <div className="action" onClick={onClose}>
        <StyledCrossIcon size="medium" />
      </div>
    </StyledBarBanner>
  );
};

BarBanner.propTypes = {
  id: PropTypes.string,
  className: PropTypes.string,
  style: PropTypes.object,
  htmlLink: PropTypes.string,
};

BarBanner.defaultProps = {
  id: undefined,
  className: undefined,
  style: undefined,
};

export default BarBanner;
