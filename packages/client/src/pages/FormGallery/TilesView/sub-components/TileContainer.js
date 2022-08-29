import React from "react";
import { withTranslation } from "react-i18next";
import PropTypes from "prop-types";
import { StyledGridWrapper, StyledTileContainer } from "../StyledTileView";

class TileContainer extends React.PureComponent {
  render() {
    const { children, id, className, style } = this.props;

    return (
      <StyledTileContainer id={id} className={className} style={style}>
        <StyledGridWrapper>{children}</StyledGridWrapper>
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

export default withTranslation(["Files", "Common"])(TileContainer);
