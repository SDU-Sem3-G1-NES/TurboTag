import { useCallback, useEffect, useMemo, useRef, useState } from 'react'
import { LessonClient, LessonDto, LessonFilter, OptionDto } from '../../api/apiClient'

export const useHomePageState = (showAllOwner: boolean, showAllStarred: boolean) => {
  const [ownerLessons, setOwnerLessons] = useState<LessonDto[]>([])
  const [starredLessons, setStarredLessons] = useState<LessonDto[]>([])
  const [loading, setLoading] = useState<boolean>(true)
  const [search, setSearch] = useState<string>('')
  const [selectedTags, setSelectedTags] = useState<OptionDto[]>([])
  const [selectedUploaderIds, setSelectedUploaderIds] = useState<OptionDto[]>([])

  const ownerId = useMemo(() => parseInt(localStorage.getItem('userId') ?? '0', 10), [])

  const [filterValues, setFilterValues] = useState<Partial<LessonFilter>>({
    ownerId,
    userId: ownerId,
    searchText: '',
    tags: []
  })

  const filterValuesJson = useMemo(() => JSON.stringify(filterValues), [filterValues])

  const filter = useMemo(() => {
    const baseFilter = JSON.parse(filterValuesJson)
    if (!showAllOwner) {
      return new LessonFilter({ ...baseFilter, pageSize: 4 })
    }
    return new LessonFilter(baseFilter)
  }, [filterValuesJson, showAllOwner])

  const starredFilter = useMemo(() => {
    const base = JSON.parse(filterValuesJson)
    if (!showAllStarred) {
      return new LessonFilter({
        ...base,
        ownerId: null,
        isStarred: true,
        userId: ownerId,
        pageSize: 4
      })
    }
    return new LessonFilter({
      ...base,
      ownerId: null,
      isStarred: true,
      userId: ownerId
    })
  }, [filterValuesJson, ownerId, showAllStarred])

  const lessonClient = useMemo(() => new LessonClient(), [])

  const loadLessons = useCallback(async () => {
    setLoading(true)
    try {
      const [all] = await Promise.all([lessonClient.getAllLessons(filter)])
      console.log('all', all)
      const [starred] = await Promise.all([lessonClient.getAllLessons(starredFilter)])

      const allList = Array.isArray(all) ? all : (all?.items ?? [])
      const starredList = Array.isArray(starred) ? starred : (starred?.items ?? [])

      setOwnerLessons(allList)
      setStarredLessons(starredList)
    } catch (error) {
      console.error('Error fetching lesson data:', error)
    } finally {
      setLoading(false)
    }
  }, [lessonClient, filter, starredFilter])

  useEffect(() => {
    loadLessons()
  }, [loadLessons, showAllOwner, showAllStarred])

  const handleSearch = useCallback((text: string) => {
    setFilterValues((prev) => {
      if (prev.searchText === text) return prev
      return { ...prev, searchText: text }
    })
  }, [])

  const isFirstRender = useRef(true)

  useEffect(() => {
    if (isFirstRender.current) {
      isFirstRender.current = false
      return
    }

    const delay = setTimeout(() => {
      handleSearch(search)
    }, 500)

    return () => clearTimeout(delay)
  }, [search, handleSearch])

  useEffect(() => {
    const tags = selectedTags
      .map((tag) => tag.value)
      .filter((v): v is string => v !== null && v !== undefined)

    setFilterValues((prev) => ({
      ...prev,
      tags
    }))
  }, [selectedTags])

  useEffect(() => {
    const uploaderIds = selectedUploaderIds
      .map((uploader) => parseInt(uploader.value as string, 10))
      .filter((v) => !isNaN(v))

    setFilterValues((prev) => ({
      ...prev,
      ownerIdInts: uploaderIds
    }))
  }, [selectedUploaderIds])

  return {
    ownerLessons,
    starredLessons,
    loading,
    search,
    setSearch,
    handleSearch,
    reload: loadLessons,
    selectedTags,
    setSelectedTags,
    selectedUploaderIds,
    setSelectedUploaderIds
  }
}
