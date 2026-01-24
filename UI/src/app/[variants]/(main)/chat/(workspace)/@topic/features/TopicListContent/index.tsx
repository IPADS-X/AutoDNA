'use client';

import { EmptyCard } from '@lobehub/ui';
import { useThemeMode } from 'antd-style';
import React, { memo } from 'react';
import { useTranslation } from 'react-i18next';
import { Flexbox } from 'react-layout-kit';

import { imageUrl } from '@/const/url';
import { useFetchTopics } from '@/hooks/useFetchTopics';
import { useChatStore } from '@/store/chat';
import { topicSelectors } from '@/store/chat/selectors';
import { useUserStore } from '@/store/user';
import { preferenceSelectors } from '@/store/user/selectors';
import { TopicDisplayMode } from '@/types/topic';

import { SkeletonList } from '../SkeletonList';
import ByTimeMode from './ByTimeMode';
import FlatMode from './FlatMode';
import SearchResult from './SearchResult';
import TopicContent from './TopicItem/TopicContent';
import DefaultContent from './TopicItem/DefaultContent';

import { start_synthesis_all, start_sequence_all } from '@/custom/routing';

const TopicListContent = memo(() => {
  const { t } = useTranslation('topic');
  const { isDarkMode } = useThemeMode();
  const [topicsInit, topicLength] = useChatStore((s) => [
    s.topicsInit,
    topicSelectors.currentTopicLength(s),
  ]);
  const [isUndefinedTopics, isInSearchMode] = useChatStore((s) => [
    topicSelectors.isUndefinedTopics(s),
    topicSelectors.isInSearchMode(s),
  ]);

  const [visible, updateGuideState, topicDisplayMode] = useUserStore((s) => [
    s.preference.guide?.topic,
    s.updateGuideState,
    preferenceSelectors.topicDisplayMode(s),
  ]);

  useFetchTopics();

  if (isInSearchMode) return <SearchResult />;

  // first time loading or has no data
  if (!topicsInit || isUndefinedTopics) return <SkeletonList />;

  return (
    <>
      {/* {topicLength === 0 && visible && (
        <Flexbox paddingInline={8}>
          <EmptyCard
            alt={t('guide.desc')}
            cover={imageUrl(`empty_topic_${isDarkMode ? 'dark' : 'light'}.webp`)}
            desc={t('guide.desc')}
            height={120}
            imageProps={{
              priority: true,
            }}
            onVisibleChange={(visible) => {
              updateGuideState({ topic: visible });
            }}
            style={{ flex: 'none', marginBottom: 12 }}
            title={t('guide.title')}
            visible={visible}
            width={200}
          />
        </Flexbox>
      )} */}
      {/* <button onClick={()=>{start_synthesis_all()}} >start_synthesis</button>
      <button onClick={()=>{start_sequence_all()}} >start_sequence</button> */}
      <FlatMode/>
      {/* {topicDisplayMode === TopicDisplayMode.ByTime ? <ByTimeMode /> : <FlatMode />} */}
    </>
  );
});

TopicListContent.displayName = 'TopicListContent';

export default TopicListContent;
