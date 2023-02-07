import React from "react";
import { withTranslation } from "react-i18next";
import PropTypes from "prop-types";
import { StyledGridWrapper, StyledTileContainer } from "../StyledTileView";
import InfiniteGrid from "./InfiniteGrid";

class TileContainer extends React.PureComponent {
  render() {
    const { children, useReactWindow, ...rest } = this.props;

    return (
      <StyledTileContainer {...rest}>
        {useReactWindow ? (
          <InfiniteGrid>{children}</InfiniteGrid>
        ) : (
          <StyledGridWrapper>{children}</StyledGridWrapper>
        )}
      </StyledTileContainer>
    );
  }
}

TileContainer.propTypes = {
  children: PropTypes.any.isRequired,
  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

TileContainer.defaultProps = {
  useReactWindow: true,
  id: "tileContainer",
};

export default withTranslation(["Files", "Common"])(TileContainer);
