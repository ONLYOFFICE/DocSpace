import React from "react";
import styled from "styled-components";
import PropTypes from "prop-types";

import TileLoader from "../TileLoader";
import RectangleLoader from "../RectangleLoader";

const StyledTilesLoader = styled.div`
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(220px, 1fr));
  width: 100%;
  grid-gap: 16px;
`;

const StyledWrapper = styled.div`
  display: grid;
  grid-template-columns: 1fr;
  grid-gap: 16px;

  .folders {
    margin-bottom: -4px;
  }
  .files {
    margin-top: 12px;
  }
  margin-right: 9px;
  @media (max-width: 1024px) {
    margin-right: 2px;
  }
`;

const TilesLoader = ({ foldersCount, filesCount, ...rest }) => {
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
          height="15px"
          width="120px"
          className="folders"
          animate={false}
          {...rest}
        />
      ) : null}
      <StyledTilesLoader>{folders}</StyledTilesLoader>
      {filesCount > 0 ? (
        <RectangleLoader
          height="15px"
          width="120px"
          className="files"
          animate={false}
          {...rest}
        />
      ) : null}
      <StyledTilesLoader>{files}</StyledTilesLoader>
    </StyledWrapper>
  );
};

TilesLoader.propTypes = {
  foldersCount: PropTypes.number,
  filesCount: PropTypes.number,
};

TilesLoader.defaultProps = {
  foldersCount: 3,
  filesCount: 3,
};

export default TilesLoader;
