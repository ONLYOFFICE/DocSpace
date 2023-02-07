import styled from "styled-components";

const StyledBreakpointWarning = styled.div`
  padding: 24px 44px 0 24px;
  display: flex;
  flex-direction: column;

  .description {
    display: flex;
    flex-direction: column;
    padding-top: 32px;
    white-space: break-spaces;
  }

  .text-breakpoint {
    font-weight: 700;
    font-size: 16px;
    line-height: 22px;
    padding-bottom: 8px;
    max-width: 348px;
  }

  .text-prompt {
    font-weight: 400;
    font-size: 12px;
    line-height: 16px;
  }

  img {
    width: 72px;
    height: 72px;
  }

  @media (min-width: 600px) {
    flex-direction: row;

    padding: 65px 0 0 104px;

    .description {
      padding: 0 0 0 32px;
    }

    img {
      width: 100px;
      height: 100px;
    }
  }
`;

export default StyledBreakpointWarning;
