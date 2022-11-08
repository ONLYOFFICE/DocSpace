import styled from "styled-components";
import { desktop, tablet } from "@docspace/components/utils/device";

const StyledWrapper = styled.div`
  width: 660px;

  @media ${tablet} {
    width: 100%;
  }
`;

const MainBlock = styled.div`
  display: flex;
  gap: 40px;
  padding: 24px;
  border-radius: 12px;
  background: linear-gradient(
      270deg,
      rgba(0, 0, 0, 0) 23.13%,
      rgba(0, 0, 0, 0.07) 50.52%,
      rgba(0, 0, 0, 0) 78.12%
    ),
    rgba(0, 0, 0, 0.05);

  .avatar {
    width: 124px;
    height: 124px;
  }

  .combos {
    display: flex;
    gap: 16px;
    flex-direction: column;

    .row {
      display: grid;
      gap: 16px;
      grid-template-columns: 75px 1fr;
    }
  }
`;

export { StyledWrapper, MainBlock };
