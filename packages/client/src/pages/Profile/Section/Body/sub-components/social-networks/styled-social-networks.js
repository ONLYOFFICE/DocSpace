import styled from "styled-components";
import { hugeMobile } from "@docspace/components/utils/device";

export const StyledWrapper = styled.div`
  display: flex;
  flex-direction: column;
  gap: 12px;

  .buttons {
    display: grid;
    grid-template-columns: 1fr 1fr;
    grid-column-gap: 20px;
    grid-row-gap: 12px;

    @media ${hugeMobile} {
      grid-template-columns: 1fr;
    }
  }
`;
