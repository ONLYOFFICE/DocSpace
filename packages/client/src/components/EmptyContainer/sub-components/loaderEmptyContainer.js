import React, { useState, useEffect } from "react";
import styled from "styled-components";
import Loaders from "@docspace/common/components/Loaders";
import { isMobileOnly, isTablet } from "react-device-detect";

const StyledLoader = styled.div`
  width: 100%;
  box-sizing: border-box;
  padding-right: ${(props) => props.viewMobile && "10px"};

  .flex {
    display: flex;
  }

  .action-section {
    margin-bottom: ${(props) =>
      !props.viewMobile && !props.viewTablet ? "12px" : "20px"};
  }

  .action-section-item:not(:last-child) {
    margin-right: 8px;
  }

  .icon-action {
    min-width: 32px;
  }

  .name-section {
    justify-content: space-between;
    margin-bottom: 21px;
  }

  .content-item {
    align-items: center;
    justify-content: space-between;
  }

  .content-item:not(:last-child) {
    margin-bottom: ${(props) =>
      !props.viewMobile && !props.viewTablet ? "17px" : "24px"};
  }

  .content-flex {
    display: flex;
    align-items: center;
    width: 100%;
  }

  .lines {
    flex-direction: column;
    margin-right: 24px;
  }

  .line {
    margin-bottom: 2px;
  }

  .line-desktop {
    margin-right: 12px;
  }

  .img {
    margin-right: 12px;
    min-width: 32px;
  }

  .menu {
    min-width: 16px;
  }
`;

const LoaderEmptyContainer = () => {
  const [viewMobile, setViewMobile] = useState(false);
  const [viewTablet, setViewTablet] = useState(false);

  useEffect(() => {
    onCheckView();
    window.addEventListener("resize", onCheckView);

    return () => window.removeEventListener("resize", onCheckView);
  }, []);

  const onCheckView = () => {
    if (isMobileOnly || window.innerWidth < 600) {
      setViewMobile(true);
    } else {
      setViewMobile(false);
    }

    if (isTablet || (window.innerWidth >= 600 && window.innerWidth <= 1024)) {
      setViewTablet(true);
    } else {
      setViewTablet(false);
    }
  };

  return (
    <StyledLoader viewMobile={viewMobile} viewTablet={viewTablet}>
      <div className="action-section flex">
        <Loaders.Rectangle className="action-section-item" height="32px" />
        <Loaders.Rectangle
          className="action-section-item icon-action"
          height="32px"
          width="32px"
        />
        <Loaders.Rectangle
          className="action-section-item icon-action"
          height="32px"
          width="32px"
        />

        {!viewMobile && !viewTablet && (
          <Loaders.Rectangle
            className="action-section-item icon-action"
            height="32px"
            width="32px"
          />
        )}
      </div>

      {!viewMobile && !viewTablet && (
        <div className="name-section flex">
          <Loaders.Rectangle height="16px" width="51px" />
          <Loaders.Rectangle height="16px" width="60px" />
          <Loaders.Rectangle height="16px" width="62px" />
          <Loaders.Rectangle height="16px" width="62px" />
          <Loaders.Rectangle height="16px" width="16px" />
        </div>
      )}

      <div className="content-section">
        <div className="content-item flex">
          <div className="content-flex">
            <Loaders.Rectangle height="32px" width="32px" className="img" />

            {!viewMobile && !viewTablet ? (
              <Loaders.Rectangle height="20px" className="line-desktop" />
            ) : (
              <div className="lines flex">
                <Loaders.Rectangle
                  className="line"
                  height="16px"
                  width={viewTablet ? "384px" : "100%"}
                />
                <Loaders.Rectangle height="16px" width="129px" />
              </div>
            )}
          </div>

          <Loaders.Rectangle height="16px" width="16px" className="menu" />
        </div>

        <div className="content-item flex">
          <div className="content-flex">
            <Loaders.Rectangle height="32px" width="32px" className="img" />

            {!viewMobile && !viewTablet ? (
              <Loaders.Rectangle height="20px" className="line-desktop" />
            ) : (
              <div className="lines flex">
                <Loaders.Rectangle
                  className="line"
                  height="16px"
                  width={viewTablet ? "384px" : "100%"}
                />
                <Loaders.Rectangle height="16px" width="129px" />
              </div>
            )}
          </div>

          <Loaders.Rectangle height="16px" width="16px" className="menu" />
        </div>

        <div className="content-item flex">
          <div className="content-flex">
            <Loaders.Rectangle height="32px" width="32px" className="img" />

            {!viewMobile && !viewTablet ? (
              <Loaders.Rectangle height="20px" className="line-desktop" />
            ) : (
              <div className="lines flex">
                <Loaders.Rectangle
                  className="line"
                  height="16px"
                  width={viewTablet ? "384px" : "100%"}
                />
                <Loaders.Rectangle height="16px" width="129px" />
              </div>
            )}
          </div>

          <Loaders.Rectangle height="16px" width="16px" className="menu" />
        </div>

        <div className="content-item flex">
          <div className="content-flex">
            <Loaders.Rectangle height="32px" width="32px" className="img" />

            {!viewMobile && !viewTablet ? (
              <Loaders.Rectangle height="20px" className="line-desktop" />
            ) : (
              <div className="lines flex">
                <Loaders.Rectangle
                  className="line"
                  height="16px"
                  width={viewTablet ? "384px" : "100%"}
                />
                <Loaders.Rectangle height="16px" width="129px" />
              </div>
            )}
          </div>

          <Loaders.Rectangle height="16px" width="16px" className="menu" />
        </div>

        <div className="content-item flex">
          <div className="content-flex">
            <Loaders.Rectangle height="32px" width="32px" className="img" />

            {!viewMobile && !viewTablet ? (
              <Loaders.Rectangle height="20px" className="line-desktop" />
            ) : (
              <div className="lines flex">
                <Loaders.Rectangle
                  className="line"
                  height="16px"
                  width={viewTablet ? "384px" : "100%"}
                />
                <Loaders.Rectangle height="16px" width="129px" />
              </div>
            )}
          </div>

          <Loaders.Rectangle height="16px" width="16px" className="menu" />
        </div>

        <div className="content-item flex">
          <div className="content-flex">
            <Loaders.Rectangle height="32px" width="32px" className="img" />

            {!viewMobile && !viewTablet ? (
              <Loaders.Rectangle height="20px" className="line-desktop" />
            ) : (
              <div className="lines flex">
                <Loaders.Rectangle
                  className="line"
                  height="16px"
                  width={viewTablet ? "384px" : "100%"}
                />
                <Loaders.Rectangle height="16px" width="129px" />
              </div>
            )}
          </div>

          <Loaders.Rectangle height="16px" width="16px" className="menu" />
        </div>

        <div className="content-item flex">
          <div className="content-flex">
            <Loaders.Rectangle height="32px" width="32px" className="img" />

            {!viewMobile && !viewTablet ? (
              <Loaders.Rectangle height="20px" className="line-desktop" />
            ) : (
              <div className="lines flex">
                <Loaders.Rectangle
                  className="line"
                  height="16px"
                  width={viewTablet ? "384px" : "100%"}
                />
                <Loaders.Rectangle height="16px" width="129px" />
              </div>
            )}
          </div>

          <Loaders.Rectangle height="16px" width="16px" className="menu" />
        </div>

        <div className="content-item flex">
          <div className="content-flex">
            <Loaders.Rectangle height="32px" width="32px" className="img" />

            {!viewMobile && !viewTablet ? (
              <Loaders.Rectangle height="20px" className="line-desktop" />
            ) : (
              <div className="lines flex">
                <Loaders.Rectangle
                  className="line"
                  height="16px"
                  width={viewTablet ? "384px" : "100%"}
                />
                <Loaders.Rectangle height="16px" width="129px" />
              </div>
            )}
          </div>

          <Loaders.Rectangle height="16px" width="16px" className="menu" />
        </div>

        {viewTablet && (
          <>
            <div className="content-item flex">
              <div className="content-flex">
                <Loaders.Rectangle height="32px" width="32px" className="img" />

                {!viewMobile && !viewTablet ? (
                  <Loaders.Rectangle height="20px" className="line-desktop" />
                ) : (
                  <div className="lines flex">
                    <Loaders.Rectangle
                      className="line"
                      height="16px"
                      width={viewTablet ? "384px" : "100%"}
                    />
                    <Loaders.Rectangle height="16px" width="129px" />
                  </div>
                )}
              </div>

              <Loaders.Rectangle height="16px" width="16px" className="menu" />
            </div>

            <div className="content-item flex">
              <div className="content-flex">
                <Loaders.Rectangle height="32px" width="32px" className="img" />

                {!viewMobile && !viewTablet ? (
                  <Loaders.Rectangle height="20px" className="line-desktop" />
                ) : (
                  <div className="lines flex">
                    <Loaders.Rectangle
                      className="line"
                      height="16px"
                      width={viewTablet ? "384px" : "100%"}
                    />
                    <Loaders.Rectangle height="16px" width="129px" />
                  </div>
                )}
              </div>

              <Loaders.Rectangle height="16px" width="16px" className="menu" />
            </div>

            <div className="content-item flex">
              <div className="content-flex">
                <Loaders.Rectangle height="32px" width="32px" className="img" />

                {!viewMobile && !viewTablet ? (
                  <Loaders.Rectangle height="20px" className="line-desktop" />
                ) : (
                  <div className="lines flex">
                    <Loaders.Rectangle
                      className="line"
                      height="16px"
                      width={viewTablet ? "384px" : "100%"}
                    />
                    <Loaders.Rectangle height="16px" width="129px" />
                  </div>
                )}
              </div>

              <Loaders.Rectangle height="16px" width="16px" className="menu" />
            </div>

            <div className="content-item flex">
              <div className="content-flex">
                <Loaders.Rectangle height="32px" width="32px" className="img" />

                {!viewMobile && !viewTablet ? (
                  <Loaders.Rectangle height="20px" className="line-desktop" />
                ) : (
                  <div className="lines flex">
                    <Loaders.Rectangle
                      className="line"
                      height="16px"
                      width={viewTablet ? "384px" : "100%"}
                    />
                    <Loaders.Rectangle height="16px" width="129px" />
                  </div>
                )}
              </div>

              <Loaders.Rectangle height="16px" width="16px" className="menu" />
            </div>
          </>
        )}

        {!viewMobile && !viewTablet && (
          <div className="content-item flex">
            <div className="content-flex">
              <Loaders.Rectangle height="32px" width="32px" className="img" />

              {!viewMobile && !viewTablet ? (
                <Loaders.Rectangle height="20px" className="line-desktop" />
              ) : (
                <div className="lines flex">
                  <Loaders.Rectangle
                    className="line"
                    height="16px"
                    width={viewTablet ? "384px" : "100%"}
                  />
                  <Loaders.Rectangle height="16px" width="129px" />
                </div>
              )}
            </div>

            <Loaders.Rectangle height="16px" width="16px" className="menu" />
          </div>
        )}
      </div>
    </StyledLoader>
  );
};

export default LoaderEmptyContainer;
