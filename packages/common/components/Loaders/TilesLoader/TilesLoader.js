import React from "react";
import styled from "styled-components";
import PropTypes from "prop-types";

import TileLoader from "../TileLoader";
import RectangleLoader from "../RectangleLoader";
import { tablet } from "@docspace/components/utils/device";

const StyledTilesLoader = styled.div`
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(216px, 1fr));
  width: 100%;
  grid-gap: 16px;
`;

const StyledWrapper = styled.div`
  display: grid;
  grid-template-columns: 1fr;
  grid-gap: 16px;

  .folders {
    margin-top: -1px;
    margin-bottom: 12px;
  }

  .files {
    margin-top: 25px;
  }

  margin-right: 3px;

  @media ${tablet} {
    margin-right: -1px;
  }
`;

const TilesLoader = ({
  foldersCount,
  filesCount,
  sectionWidth,
  withTitle,
  ...rest
}) => {
  const folders = [];
  const files = [];

  for (let i = 0; i < foldersCount; i++) {
    folders.push(<TileLoader isFolder key={`tile-loader-${i}`} {...rest} />);
  }

  for (let i = 0; i < filesCount; i++) {
    files.push(<TileLoader key={`files-loader-${i}`} {...rest} />);
  }

  return (
    <StyledWrapper>
      {foldersCount > 0 ? (
        <RectangleLoader
          height="22px"
          width="57px"
          className="folders"
          animate
          {...rest}
        />
      ) : null}
      <StyledTilesLoader>{folders}</StyledTilesLoader>

      {filesCount > 0
        ? withTitle && (
            <RectangleLoader
              height="22px"
              width="35px"
              className="files"
              animate
              {...rest}
            />
          )
        : null}
      <StyledTilesLoader>{files}</StyledTilesLoader>
    </StyledWrapper>
  );
};

TilesLoader.propTypes = {
  foldersCount: PropTypes.number,
  filesCount: PropTypes.number,
};

TilesLoader.defaultProps = {
  foldersCount: 2,
  filesCount: 8,
  withTitle: true,
};

export default TilesLoader;
