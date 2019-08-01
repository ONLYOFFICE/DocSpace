import React from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import { Backdrop, NewPageLayout as NPL } from "asc-web-components";
import {
  ArticleHeaderContent,
  ArticleBodyContent,
  ArticleMainButtonContent
} from "../../Article";
import {
  SectionHeaderContent,
  SectionBodyContent,
  SectionFilterContent,
  SectionPagingContent
} from "./Section";
import { setSelected } from "../../../store/people/actions";

class Home extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      isHeaderVisible: false,
      isHeaderIndeterminate: false,
      isHeaderChecked: false,
      isBackdropVisible: false,
      isArticleVisible: false,
      isArticlePinned: false
    };
  }

  renderGroupButtonMenu = () => {
    const { users, selection, selected, setSelected } = this.props;

    const headerVisible = selection.length > 0;
    const headerIndeterminate =
      headerVisible && selection.length > 0 && selection.length < users.length;
    const headerChecked = headerVisible && selection.length === users.length;

    console.log(`renderGroupButtonMenu()
      headerVisible=${headerVisible} 
      headerIndeterminate=${headerIndeterminate} 
      headerChecked=${headerChecked}
      selection.length=${selection.length}
      users.length=${users.length}
      selected=${selected}`);

    let newState = {};

    if (headerVisible || selected === "close") {
      newState.isHeaderVisible = headerVisible;
      if (selected === "close") {
        setSelected({ selected: "none" });
      }
    }

    newState.isHeaderIndeterminate = headerIndeterminate;
    newState.isHeaderChecked = headerChecked;

    this.setState(newState);
  };

  componentDidUpdate(prevProps) {
    if (this.props.selection !== prevProps.selection) {
      this.renderGroupButtonMenu();
    }
  }

  onBackdropClick = () => {
    this.setState({
      isBackdropVisible: false,
      isArticleVisible: false,
      isArticlePinned: false
    });
  };

  onPinArticle = () => {
    this.setState({
      isBackdropVisible: false,
      isArticleVisible: true,
      isArticlePinned: false
    });
  };

  onUnpinArticle = () => {
    this.setState({
      isBackdropVisible: true,
      isArticleVisible: true,
      isArticlePinned: false
    });
  };

  onShowArticle = () => {
    this.setState({
      isBackdropVisible: true,
      isArticleVisible: true,
      isArticlePinned: false
    });
  };

  onSectionHeaderContentCheck = (checked) => {
      this.props.setSelected(checked ? "all" : "none");
  }

  onSectionHeaderContentSelect = (selected) => {
    this.props.setSelected(selected);
  }

  onClose = () => {
    const { selection, setSelected } = this.props;

    if (!selection.length) {
      setSelected("none");
      this.setState({ isHeaderVisible: false });
    } else {
      setSelected("close");
    }
  }

  render() {
    
    const {
      isHeaderVisible,
      isHeaderIndeterminate,
      isHeaderChecked,
      selected,
      isBackdropVisible,
      isArticleVisible,
      isArticlePinned
    } = this.state;
    return (
      <>
        <Backdrop visible={isBackdropVisible} onClick={this.onBackdropClick} />
        <NPL.Article visible={isArticleVisible} pinned={isArticlePinned}>
          <NPL.ArticleHeader visible={isArticlePinned}>
            <ArticleHeaderContent />
          </NPL.ArticleHeader>
          <NPL.ArticleMainButton>
            <ArticleMainButtonContent />
          </NPL.ArticleMainButton>
          <NPL.ArticleBody>
            <ArticleBodyContent />
          </NPL.ArticleBody>
          <NPL.ArticlePinPanel
            pinned={isArticlePinned}
            pinText="Pin this panel"
            onPin={this.onPinArticle}
            unpinText="Unpin this panel"
            onUnpin={this.onUnpinArticle}
          />
        </NPL.Article>
        <NPL.Section>
          <NPL.SectionHeader>
            <SectionHeaderContent
              isHeaderVisible={isHeaderVisible}
              isHeaderIndeterminate={isHeaderIndeterminate}
              isHeaderChecked={isHeaderChecked}
              onCheck={this.onSectionHeaderContentCheck}
              onSelect={this.onSectionHeaderContentSelect}
              onClose={this.onClose}
            />
          </NPL.SectionHeader>
          <NPL.SectionFilter>
            <SectionFilterContent />
          </NPL.SectionFilter>
          <NPL.SectionBody>
            <SectionBodyContent
              selected={selected}
              onChange={this.onRowChange}
            />
          </NPL.SectionBody>
          <NPL.SectionPaging>
            <SectionPagingContent />
          </NPL.SectionPaging>
          <NPL.SectionToggler
            visible={!isArticlePinned}
            onClick={this.onShowArticle}
          />
        </NPL.Section>
      </>
    );
  }
}

Home.propTypes = {
  users: PropTypes.array.isRequired,
  history: PropTypes.object.isRequired,
  isLoaded: PropTypes.bool
};

function mapStateToProps(state) {
  return {
    users: state.people.users,
    selection: state.people.selection,
    selected: state.people.selected,
    isLoaded: state.auth.isLoaded
  };
}

export default connect(
  mapStateToProps,
  { setSelected }
)(withRouter(Home));
