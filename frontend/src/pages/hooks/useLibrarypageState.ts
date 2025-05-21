import { useCallback, useEffect, useMemo, useRef, useState } from 'react'
import {
  isPagedResult,
  LessonClient,
  LessonDto,
  LessonFilter,
  OptionDto
} from '../../api/apiClient'

export const useLibraryPageState = () => {
  const [lessons, setLessons] = useState<LessonDto[]>([])
  const [loading, setLoading] = useState<boolean>(true)
  const [search, setSearch] = useState<string>('')
  const [selectedTags, setSelectedTags] = useState<OptionDto[]>([])
  const [selectedUploaderIds, setSelectedUploaderIds] = useState<OptionDto[]>([])
  const [page, setPage] = useState<number>(1)
  const [totalCount, setTotalCount] = useState<number>(0)
  const [totalPages, setTotalPages] = useState<number>(1)
  const pageSize = 10

  const [filterValues, setFilterValues] = useState<Partial<LessonFilter>>({
    searchText: '',
    userId: parseInt(localStorage.getItem('userId') ?? '0', 10),
    tags: [],
    pageSize: pageSize,
    pageNumber: 1
  })

  const filter = useMemo(() => {
    return new LessonFilter({
      ...filterValues,
      pageSize: pageSize,
      pageNumber: page
    })
  }, [filterValues, page])

  const lessonClient = useMemo(() => new LessonClient(), [])

  const loadLessons = useCallback(async () => {
    setLoading(true)
    try {
      const result = await lessonClient.getAllLessons(filter)

      if (isPagedResult<LessonDto>(result)) {
        setLessons(result.items)
        setTotalCount(result.totalCount)
        setTotalPages(result.totalPages ?? 1)
      } else {
        setLessons([])
        setTotalCount(0)
        setTotalPages(1)
      }
    } catch (error) {
      console.error('Error fetching lessons:', error)
    } finally {
      setLoading(false)
    }
  }, [filter, lessonClient])

  useEffect(() => {
    loadLessons()
  }, [filter, loadLessons])

  const handleSearch = useCallback((text: string) => {
    setPage(1)
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

    setPage(1)
    setFilterValues((prev) => ({
      ...prev,
      tags
    }))
  }, [selectedTags])

  useEffect(() => {
    const uploaderIds = selectedUploaderIds
      .map((uploader) => parseInt(uploader.value as string, 10))
      .filter((v) => !isNaN(v))

    setPage(1)
    setFilterValues((prev) => ({
      ...prev,
      ownerIdInts: uploaderIds
    }))
  }, [selectedUploaderIds])

  return {
    lessons,
    loading,
    search,
    setSearch,
    handleSearch,
    reload: loadLessons,
    selectedTags,
    setSelectedTags,
    selectedUploaderIds,
    setSelectedUploaderIds,
    page,
    setPage,
    totalCount,
    totalPages
  }
}
