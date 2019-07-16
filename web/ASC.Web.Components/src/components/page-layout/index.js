import React from 'react'
import PropTypes from 'prop-types'
import styled from 'styled-components'
import device from '../device'
import Backdrop from '../backdrop'
import { Icons } from '../icons'
import { Scrollbars } from 'react-custom-scrollbars';

const StyledArticle = styled.article`
  padding: 0 16px;
  background: #F8F9F9;
  display: flex;
  flex-direction: column;
  width: 264px;
  min-width: 264px;
  transition: width .3s ease-in-out;
  overflow: hidden auto;

  @media ${device.tablet} {
    ${props => props.visible
      ? props.pinned
          ? `
            display: flex;
            width: 240px;
            min-width: 240px;
          `
          : `
            width: 240px;
            min-width: 240px;
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
    height: 100%;

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
  overflow: hidden auto;
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
    padding: 12px 13px 14px 15px;
    box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
    border-radius: 48px;
    cursor: pointer;
  }
`;

const renderArticleThumbVertical = ({ style, ...props }) => 
  <div {...props} style={{ ...style, backgroundColor: 'rgba(0, 0, 0, 0.1)', width: '2px', marginLeft: '2px', borderRadius: 'inherit'}}/>

const renderArticleThumbHorizontal = ({ style, ...props }) => 
  <div {...props} style={{ ...style, backgroundColor: 'rgba(0, 0, 0, 0.1)', height: '2px', marginTop: '2px', borderRadius: 'inherit'}}/>

const renderSectionThumbVertical = ({ style, ...props }) => 
  <div {...props} style={{ ...style, backgroundColor: 'rgba(0, 0, 0, 0.1)', width: '6px', borderRadius: 'inherit'}}/>

const renderSectionThumbHorizontal = ({ style, ...props }) => 
  <div {...props} style={{ ...style, backgroundColor: 'rgba(0, 0, 0, 0.1)', height: '6px', borderRadius: 'inherit'}}/>

class PageLayout extends React.Component {
  constructor(props) {
    super(props);
    this.state = this.mapPropsToState(props);
  };

  componentDidUpdate(prevProps, prevState) {
    if (this.hasChanges(this.props,prevProps)) {
      this.setState(this.mapPropsToState(this.props));
    }
  }

  hasChanges = (currentProps, prevProps) => {
    return currentProps.articleHeaderContent != prevProps.articleHeaderContent ||
    currentProps.articleBodyContent != prevProps.articleBodyContent ||
    currentProps.sectionHeaderContent != prevProps.sectionHeaderContent ||
    currentProps.sectionBodyContent != prevProps.sectionBodyContent;
  }

  mapPropsToState = (props) => {
    let isArticleHeaderAvailable = !!props.articleHeaderContent ,
        isArticleBodyAvailable = !!props.articleBodyContent,
        isArticleAvailable = isArticleHeaderAvailable || isArticleBodyAvailable,
        isSectionHeaderAvailable = !!props.sectionHeaderContent,
        isSectionBodyAvailable = !!props.sectionBodyContent,
        isSectionAvailable = isSectionHeaderAvailable || isSectionBodyAvailable || isArticleAvailable,
        isBackdropAvailable = isArticleAvailable;

    let newState = {
      isBackdropAvailable: isBackdropAvailable,
      isArticleAvailable: isArticleAvailable,
      isArticleHeaderAvailable: isArticleHeaderAvailable,
      isArticleBodyAvailable: isArticleBodyAvailable,
      isSectionAvailable: isSectionAvailable,
      isSectionHeaderAvailable: isSectionHeaderAvailable,
      isSectionBodyAvailable: isSectionBodyAvailable,

      isBackdropVisible: props.isBackdropVisible,
      isArticleVisible: props.isArticleVisible,
      isArticlePinned: props.isArticlePinned,

      articleHeaderContent: props.articleHeaderContent,
      articleBodyContent: props.articleBodyContent,
      sectionHeaderContent: props.sectionHeaderContent,
      sectionBodyContent: props.sectionBodyContent
    };

    return newState;
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
        {
          this.state.isBackdropAvailable &&
          <Backdrop visible={this.state.isBackdropVisible} onClick={this.backdropClick}/>
        }
        {
          this.state.isArticleAvailable &&
          <StyledArticle visible={this.state.isArticleVisible} pinned={this.state.isArticlePinned}>
            {
              this.state.isArticleHeaderAvailable &&
              <StyledArticleHeader visible={this.state.isArticlePinned}>{this.state.articleHeaderContent}</StyledArticleHeader>
            }
            {
              this.state.isArticleBodyAvailable &&
              <StyledArticleBody>
                <Scrollbars renderThumbVertical={renderArticleThumbVertical} renderThumbHorizontal={renderArticleThumbHorizontal}>
                  {this.state.articleBodyContent}
                </Scrollbars>
              </StyledArticleBody>
            }
            {
              this.state.isArticleBodyAvailable &&
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
            }
          </StyledArticle>
        }
        {
          this.state.isSectionAvailable &&
          <StyledSection>
            {
              this.state.isSectionHeaderAvailable &&
              <StyledSectionHeader>{this.state.sectionHeaderContent}</StyledSectionHeader>
            }
            {
              this.state.isSectionBodyAvailable &&
              <StyledSectionBody>
                <Scrollbars renderThumbVertical={renderSectionThumbVertical} renderThumbHorizontal={renderSectionThumbHorizontal}>
                  {this.state.sectionBodyContent}
                </Scrollbars>
              </StyledSectionBody>
            }
            {
              this.state.isArticleAvailable &&
              <StyledSectionPagingPanel visible={!this.state.isArticlePinned}>
                <div onClick={this.showArticle}>
                  <Icons.CatalogButtonIcon size="scale"/>
                </div>
              </StyledSectionPagingPanel>
            }
          </StyledSection>
        }
      </>
    )
  }
}

PageLayout.propTypes = {
  isBackdropVisible: PropTypes.bool,
  isArticleVisible: PropTypes.bool,
  isArticlePinned: PropTypes.bool,

  articleHeaderContent: PropTypes.oneOfType([PropTypes.arrayOf(PropTypes.node), PropTypes.node]),
  articleBodyContent: PropTypes.oneOfType([PropTypes.arrayOf(PropTypes.node), PropTypes.node]),
  sectionHeaderContent: PropTypes.oneOfType([PropTypes.arrayOf(PropTypes.node), PropTypes.node]),
  sectionBodyContent: PropTypes.oneOfType([PropTypes.arrayOf(PropTypes.node), PropTypes.node])
}

PageLayout.defaultProps = {
  isBackdropVisible: false,
  isArticleVisible: false,
  isArticlePinned: false
}

export default PageLayout