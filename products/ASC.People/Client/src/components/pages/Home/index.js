import React, { useState } from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import _ from "lodash";
import { Backdrop, NewPageLayout as NPL } from "asc-web-components";
import {ArticleHeaderContent, ArticleBodyContent, ArticleMainButtonContent} from '../../Article';
import {SectionHeaderContent, SectionBodyContent, SectionFilterContent, SectionPagingContent} from './Section';

let selection = [];

const Home = ({users}) => {
  console.log("Home render");
  
  const [isHeaderVisible, toggleHeaderVisible] = useState(false);
  const [isHeaderIndeterminate, toggleHeaderIndeterminate] = useState(false);
  const [isHeaderChecked, toggleHeaderChecked] = useState(false);
  const [selected, setSelected] = useState("none");

  const [isBackdropVisible, setIsBackdropVisible] = useState(false);
  const [isArticleVisible, setIsArticleVisible] = useState(false);
  const [isArticlePinned, setIsArticlePinned] = useState(false);

  const renderGroupButtonMenu = () => {
    const headerVisible = selection.length > 0;
    const headerIndeterminate = headerVisible && selection.length > 0 && selection.length < users.length;
    const headerChecked = headerVisible && selection.length === users.length;

    console.log(`renderGroupButtonMenu()
      headerVisible=${headerVisible} 
      headerIndeterminate=${headerIndeterminate} 
      headerChecked=${headerChecked}
      selection.length=${selection.length}
      users.length=${users.length}
      selected=${selected}`);

    if(headerVisible || selected === "close") {
      toggleHeaderVisible(headerVisible);
      if(selected === "close")
        setSelected("none");
    }

    toggleHeaderIndeterminate(headerIndeterminate);
    toggleHeaderChecked(headerChecked);
  };

  const onRowChange = (checked, data) => {
    /*console.log(`onBodySelect 
        row.checked=${checked}`,
          data,
          selection);*/

    const id = _.result(
      _.find(selection, function(obj) {
        return obj.id === data.id;
      }),
      "id"
    );
    if (checked && !id) {
      selection.push(data);
    } else if (id) {
      selection = _.filter(selection, function(obj) {
        return obj.id !== id;
      });
    }

    renderGroupButtonMenu();
  };

  const onBackdropClick = () => {
    setIsBackdropVisible(false);
    setIsArticleVisible(false);
    setIsArticlePinned(false);
  };

  const onPinArticle = () => {
    setIsBackdropVisible(false);
    setIsArticleVisible(true);
    setIsArticlePinned(true);
  };

  const onUnpinArticle = () => {
    setIsBackdropVisible(true);
    setIsArticleVisible(true);
    setIsArticlePinned(false);
  };

  const onShowArticle = () => {
    setIsBackdropVisible(true);
    setIsArticleVisible(true);
    setIsArticlePinned(false);
  };

  return (
    <>
      <Backdrop visible={isBackdropVisible} onClick={onBackdropClick} />
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
        <NPL.ArticlePinPanel pinned={isArticlePinned} pinText="Pin this panel" onPin={onPinArticle} unpinText="Unpin this panel" onUnpin={onUnpinArticle} />
      </NPL.Article>
      <NPL.Section>
        <NPL.SectionHeader>
          <SectionHeaderContent
            isHeaderVisible={isHeaderVisible}
            isHeaderIndeterminate={isHeaderIndeterminate}
            isHeaderChecked={isHeaderChecked}
            onCheck={checked => {
              /*console.log(`SectionHeaderContent onCheck 
                  selection.length=${selection.length}`);*/
              setSelected(checked ? "all" : "none");
            }}
            onSelect={(selected) => {
              /*console.log(`SectionHeaderContent onSelect 
                selected=${selected}`);*/
              setSelected(selected);
            }}
            onClose={() => {
              /*console.log('SectionHeaderContent onClose');*/
              if(!selection.length) {
                setSelected("none");
                toggleHeaderVisible(false);
              }
              else {
                setSelected("close");
              }
            }}
          />
        </NPL.SectionHeader>
        <NPL.SectionFilter>
          <SectionFilterContent />
        </NPL.SectionFilter>
        <NPL.SectionBody>
          <SectionBodyContent
            users={users}
            selected={selected}
            onRowChange={onRowChange}
          />
        </NPL.SectionBody>
        <NPL.SectionPaging>
          <SectionPagingContent />
        </NPL.SectionPaging>
        <NPL.SectionToggler visible={!isArticlePinned} onClick={onShowArticle} />
      </NPL.Section>
    </>
  );
};

Home.propTypes = {
  users: PropTypes.array.isRequired,
  history: PropTypes.object.isRequired,
  isLoaded: PropTypes.bool
};

function mapStateToProps(state) {
  return {
    users: state.people.users,
    isLoaded: state.auth.isLoaded
  };
}

export default connect(mapStateToProps)(withRouter(Home));