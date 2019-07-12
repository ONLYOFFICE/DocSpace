import React from 'react'
import PropTypes from 'prop-types'
import styled from 'styled-components'
import device from '../device'
import Backdrop from '../backdrop'
import { Icons } from '../icons'

const StyledArticle = styled.article`
  padding: 0 16px;
  background: #F8F9F9;
  display: flex;
  flex-direction: column;
  width: 240px;
  transition: width .3s ease-in-out;

  @media ${device.tablet} {
    ${props => props.visible
      ? props.pinned
          ? `
            display: flex;
            width: 240px;
          `
          : `
            position: fixed;
            height: 100%;
            top: 0;
            left: 0;
            z-index: 400;
          `
      : `
        display: none;
        width: 0px;
      `
    }
    }
`;

const StyledArticleHeader = styled.div`
  border-bottom: 1px solid #ECEEF1;
  font-weight: bold;
  font-size: 27px;
  line-height: 56px;
  height: 56px;

  @media ${device.tablet} {
    display: ${props => props.visible ? 'block' : 'none'};
  }
`;

const StyledArticleBody = styled.div`
  margin: 16px 0;
  outline: 1px dotted;
  flex-grow: 1;
`;

const StyledArticlePinPanel = styled.div`
  border-top: 1px solid #ECEEF1;
  height: 56px;
  line-height: 56px;
  display: none;

  @media ${device.tablet} {
    display: block;
  }

  @media ${device.mobile} {
    display: none;
  }

  div {
    display: flex;
    align-items: center;
    cursor: pointer;
    user-select: none;

    span {
      margin-left: 8px;
    }
  }
`;

const StyledSection = styled.section`
  padding: 0 16px;
  flex-grow: 1;
  display: flex;
  flex-direction: column;
`;

const StyledSectionHeader = styled.div`
  border-bottom: 1px solid #ECEEF1;
  font-weight: bold;
  font-size: 21px;
  line-height: 56px;
  height: 56px;
`;

const StyledSectionBody = styled.div`
  margin: 16px 0;
  outline: 1px dotted;
  flex-grow: 1;
`;

const StyledSectionPagingPanel = styled.div`
  height: 64px;
  display: none;

  @media ${device.tablet} {
    display: ${props => props.visible ? 'block' : 'none'};
  }

  div {
    width: 48px;
    height: 48px;
    padding: 14px 13px 13px 14px;
    box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
    border-radius: 48px;
    cursor: pointer;
  }
`;

class PageLayout extends React.Component {

  constructor(props) {
    super(props);

    this.state = {
      isBackdropVisible: props.isBackdropVisible,
      isArticleVisible: props.isArticleVisible,
      isArticlePinned: props.isArticlePinned
    };
  }

  backdropClick = () => {
    this.setState({
      isBackdropVisible: false,
      isArticleVisible: false,
      isArticlePinned: false
    });
  };

  pinArticle = () => {
    this.setState({
      isBackdropVisible: false,
      isArticlePinned: true,
      isArticleVisible: true 
    });
  };

  unpinArticle = () => {
    this.setState({
      isBackdropVisible: true,
      isArticlePinned: false,
      isArticleVisible: true 
    });
  };

  showArticle = () => {
    this.setState({
      isBackdropVisible: true,
      isArticleVisible: true,
      isArticlePinned: false
    });
  };

  render() {
    return (
      <>
        <Backdrop visible={this.state.isBackdropVisible} onClick={this.backdropClick}/>
        <StyledArticle visible={this.state.isArticleVisible} pinned={this.state.isArticlePinned}>
          <StyledArticleHeader visible={this.state.isArticlePinned}/>
          <StyledArticleBody/>
          <StyledArticlePinPanel>
            {
              this.state.isArticlePinned
                ? <div onClick={this.unpinArticle}>
                    <Icons.CatalogUnpinIcon size="medium"/>
                    <span>Unpin this panel</span>
                  </div>
                : <div onClick={this.pinArticle}>
                    <Icons.CatalogPinIcon size="medium"/>
                    <span>Pin this panel</span>
                  </div>
            }
          </StyledArticlePinPanel>
        </StyledArticle>
        <StyledSection>
          <StyledSectionHeader/>
          <StyledSectionBody/>
          <StyledSectionPagingPanel visible={!this.state.isArticlePinned}>
            <div onClick={this.showArticle}>
              <Icons.CatalogButtonIcon size="scale"/>
            </div>
          </StyledSectionPagingPanel>
        </StyledSection>
      </>
    )
  }
}

PageLayout.propTypes = {
  isBackdropVisible: PropTypes.bool,
  isArticleVisible: PropTypes.bool,
  isArticlePinned: PropTypes.bool
}

PageLayout.defaultProps = {
  isBackdropVisible: false,
  isArticleVisible: false,
  isArticlePinned: false
}

export default PageLayout