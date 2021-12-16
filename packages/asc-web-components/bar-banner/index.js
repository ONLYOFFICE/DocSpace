import React, { useState } from "react";

import { StyledBarBanner, StyledCrossIcon } from "./styled-bar-banner";

const BarBanner = (props) => {
  const { html } = props;

  const [open, setOpen] = useState(true);

  const onClose = () => setOpen(false);

  return (
    <StyledBarBanner open={open}>
      {html ? (
        <iframe
          height="60px"
          width="95%"
          src={html}
          frameBorder="0"
          align="left"
          scrolling="no"
        >
          test
        </iframe>
      ) : (
        ""
      )}
      <div className="action" onClick={onClose}>
        <StyledCrossIcon size="medium" />
      </div>
    </StyledBarBanner>
  );
};

export default BarBanner;
