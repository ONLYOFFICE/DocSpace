import styled from "styled-components";

import Loaders from "@docspace/common/components/Loaders";

const StyledLoader = styled.div`
  max-width: 700px;
  width: 100%;
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(293px, 1fr));
  gap: 20px;
`;

const ThirdPartyLoader = () => {
  const rectangles = new Array(6).fill(0);

  return (
    <StyledLoader>
      {rectangles.map((_) => (
        <Loaders.Rectangle height="120px" borderRadius="6px" />
      ))}
    </StyledLoader>
  );
};

export default ThirdPartyLoader;
