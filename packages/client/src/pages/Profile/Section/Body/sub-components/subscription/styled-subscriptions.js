import styled from "styled-components";
import { smallTablet } from "@docspace/components/utils/device";

export const StyledWrapper = styled.div`
  display: flex;
  flex-direction: column;
  gap: 12px;

  button {
    max-width: 195px;

    @media ${smallTablet} {
      max-width: 100%;
      width: 100%;
    }
  }
`;
