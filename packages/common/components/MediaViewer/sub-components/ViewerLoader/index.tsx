import React from "react";
import styled from "styled-components";

const StyledLoaderWrapper = styled.div`
  position: fixed;
  inset: 0;

  width: 100%;
  height: 100%;

  display: flex;
  justify-content: center;
  align-items: center;

  background-color: rgba(0, 0, 0, 0.4);
`;

const StyledLoader = styled.div`
  width: 48px;
  height: 48px;
  border: 4px solid #fff;
  border-bottom-color: transparent;
  border-radius: 50%;
  display: inline-block;
  box-sizing: border-box;
  animation: rotation 1s linear infinite;

  @keyframes rotation {
    0% {
      transform: rotate(0deg);
    }
    100% {
      transform: rotate(360deg);
    }
  }
`;

type ViewerLoader = {
  isLoading: boolean;
  isError?: boolean;
};

export default function ViewerLoader({ isLoading, isError }: ViewerLoader) {
  if (!isLoading || isError) return <></>;

  return (
    <StyledLoaderWrapper>
      <StyledLoader />
    </StyledLoaderWrapper>
  );
}
