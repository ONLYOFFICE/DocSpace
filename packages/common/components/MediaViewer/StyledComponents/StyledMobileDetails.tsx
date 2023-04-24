import styled from "styled-components";

const StyledMobileDetails = styled.div`
  z-index: 307;
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  height: 53px;
  display: flex;
  justify-content: center;
  align-items: center;
  background: linear-gradient(
    0deg,
    rgba(0, 0, 0, 0) 0%,
    rgba(0, 0, 0, 0.8) 100%
  );

  svg {
    path {
      fill: #fff;
    }
  }

  .mobile-close {
    position: fixed;
    left: 21px;
    top: 22px;
  }

  .mobile-context {
    position: fixed;
    right: 22px;
    top: 22px;
  }

  .title {
    font-weight: 600;
    margin-top: 6px;
    width: calc(100% - 100px);
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
  }
`;

export default StyledMobileDetails;
